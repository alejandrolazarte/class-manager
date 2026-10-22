using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record ReplyToAnnouncementRequest(bool IsGoing);

public sealed record ReplyToAnnouncementCommand(long AnnouncementId, bool IsGoing) : ICommand;

public sealed class ReplyToAnnouncementUseCase(
    IStudentAppAccess studentAppAccess,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IAnnouncementRepository announcementRepository,
    IBusinessCalendarService businessCalendar,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<ReplyToAnnouncementCommand, bool>
{
    private const string NoAccessMessage = "This account is not linked to a client.";
    private const string NoAccessCode = "student.no_access";
    private const string NotFoundMessage = "The announcement doesn't exist.";

    public async Task<Result<bool>> ExecuteAsync(ReplyToAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return Result.Unauthorized<bool>(NoAccessMessage, NoAccessCode);
        }

        var now = timeProvider.GetUtcNow();
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var studentIds = (await studentRepository.ListByClientAsync(access.ClientId, cancellationToken)).Select(student => student.Id);
        var enrolledClassGroupIds = await StudentAnnouncements.EnrolledClassGroupIdsAsync(enrollmentRepository, studentIds, today, cancellationToken);
        var announcement = await announcementRepository.FindForUpdateAsync(command.AnnouncementId, cancellationToken);
        if (announcement is null
            || !announcement.IsPublishedBy(now)
            || !announcement.IsVisibleOn(today)
            || !announcement.IsFor(enrolledClassGroupIds))
        {
            return Result.NotFound<bool>(NotFoundMessage, AnnouncementErrorCodes.NotFound);
        }

        var reply = await announcementRepository.FindReplyForUpdateAsync(announcement.Id, access.ClientId, cancellationToken);
        if (reply is null)
        {
            var created = AnnouncementReply.Create(announcement, access.ClientId, command.IsGoing, now);
            if (created.IsFailure)
            {
                return created.Error!;
            }

            announcementRepository.AddReply(created.Value!);
        }
        else
        {
            reply.Change(command.IsGoing, now);
        }

        if ((await announcementRepository.ListViewsAsync([announcement.Id], cancellationToken)).All(view => view.ClientId != access.ClientId))
        {
            announcementRepository.AddView(AnnouncementView.Create(announcement.Id, access.ClientId, now));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
