using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.UseCases.Announcements;

public sealed record UpdateAnnouncementRequest(
    string? Title,
    string? Body,
    AnnouncementCategory? Category = null,
    IReadOnlyList<Guid>? ClassGroupIds = null,
    bool IsPinned = false,
    DateOnly? VisibleUntil = null,
    DateTimeOffset? PublishOn = null)
{
    public UpdateAnnouncementCommand ToCommand(Guid announcementId) =>
        new(announcementId, Title, Body, Category, ClassGroupIds, IsPinned, VisibleUntil, PublishOn);
}

public sealed record UpdateAnnouncementCommand(
    Guid AnnouncementId,
    string? Title,
    string? Body,
    AnnouncementCategory? Category = null,
    IReadOnlyList<Guid>? ClassGroupIds = null,
    bool IsPinned = false,
    DateOnly? VisibleUntil = null,
    DateTimeOffset? PublishOn = null) : ICommand;

public sealed class UpdateAnnouncementUseCase(
    IAnnouncementRepository announcementRepository,
    IClassGroupRepository classGroupRepository,
    IAnnouncementResponseService responses,
    IUnitOfWork unitOfWork,
    IStudentAppNotificationService studentAppNotifications,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<UpdateAnnouncementCommand, AnnouncementResponse>
{
    public async Task<Result<AnnouncementResponse>> ExecuteAsync(UpdateAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.FindForUpdateAsync(command.AnnouncementId, cancellationToken);
        if (announcement is null)
        {
            return AnnouncementFailures.NotFound();
        }

        var now = timeProvider.GetUtcNow();
        var wasScheduled = !announcement.IsPublishedBy(now);
        var staysScheduled = wasScheduled && command.PublishOn > now;
        if (staysScheduled && command.IsPinned)
        {
            return AnnouncementFailures.ScheduledCannotBePinned();
        }

        foreach (var classGroupId in (command.ClassGroupIds ?? []).Except(announcement.ClassGroupIds))
        {
            if (await classGroupRepository.GetByIdAsync(classGroupId, cancellationToken) is null)
            {
                return AnnouncementFailures.ClassGroupNotFound();
            }
        }

        var edited = announcement.Edit(
            command.Title,
            command.Body,
            command.Category ?? announcement.Category,
            command.ClassGroupIds,
            command.VisibleUntil,
            await businessCalendar.TodayAsync(cancellationToken));
        if (edited.IsFailure)
        {
            return edited.Error!;
        }

        var previousPublishedAt = announcement.PublishedAt;
        if (wasScheduled)
        {
            announcement.Reschedule(staysScheduled ? command.PublishOn!.Value : now);
        }

        if (command.IsPinned && !announcement.IsPinned)
        {
            await AnnouncementPins.UnpinCurrentAsync(announcementRepository, unitOfWork, cancellationToken);
            announcement.Pin();
        }
        else if (!command.IsPinned)
        {
            announcement.Unpin();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        if (announcement.PublishedAt != previousPublishedAt)
        {
            await studentAppNotifications.AnnouncementRescheduledAsync(announcement, cancellationToken);
        }

        return await responses.FromAsync(announcement, cancellationToken);
    }
}
