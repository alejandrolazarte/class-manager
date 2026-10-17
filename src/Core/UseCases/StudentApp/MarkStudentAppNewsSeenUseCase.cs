using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record MarkStudentAppNewsSeenCommand : ICommand;

public sealed class MarkStudentAppNewsSeenUseCase(
    IStudentAppAccess studentAppAccess,
    IClientAccountRepository clientAccountRepository,
    IStudentRepository studentRepository,
    IEnrollmentRepository enrollmentRepository,
    IAnnouncementRepository announcementRepository,
    IBusinessCalendarService businessCalendar,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<MarkStudentAppNewsSeenCommand, bool>
{
    private const string NoAccessMessage = "This account is not linked to a client.";
    private const string NoAccessCode = "student.no_access";

    public async Task<Result<bool>> ExecuteAsync(MarkStudentAppNewsSeenCommand command, CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        var account = access is null ? null : await clientAccountRepository.FindByAccountForUpdateAsync(access.AccountId, cancellationToken);
        if (access is null || account is null)
        {
            return Result.Unauthorized<bool>(NoAccessMessage, NoAccessCode);
        }

        var now = timeProvider.GetUtcNow();
        account.MarkNewsSeen(now);
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var studentIds = (await studentRepository.ListByClientAsync(access.ClientId, cancellationToken)).Select(student => student.Id);
        var enrolledClassGroupIds = await StudentAnnouncements.EnrolledClassGroupIdsAsync(enrollmentRepository, studentIds, today, cancellationToken);
        var announcements = await StudentAnnouncements.VisibleAsync(announcementRepository, enrolledClassGroupIds, today, now, cancellationToken);
        var viewedIds = (await announcementRepository.ListViewsAsync([.. announcements.Select(announcement => announcement.Id)], cancellationToken))
            .Where(view => view.ClientId == access.ClientId)
            .Select(view => view.AnnouncementId)
            .ToHashSet();
        foreach (var announcement in announcements.Where(announcement => !viewedIds.Contains(announcement.Id)))
        {
            announcementRepository.AddView(AnnouncementView.Create(announcement.Id, access.ClientId, now));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
