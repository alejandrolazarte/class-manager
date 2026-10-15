using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Infrastructure.BackgroundTasks;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class PublishScheduledAnnouncementBackgroundTaskHandler(
    AppDbContext context,
    IStudentAppNotificationService studentAppNotifications,
    TimeProvider timeProvider)
    : IBackgroundTaskHandler<PublishScheduledAnnouncementBackgroundTaskCommand>
{
    public async Task HandleAsync(PublishScheduledAnnouncementBackgroundTaskCommand command, CancellationToken cancellationToken)
    {
        var announcement = await context.Announcements.AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == command.AnnouncementId, cancellationToken);
        if (announcement is null || !announcement.IsPublishedBy(timeProvider.GetUtcNow()))
        {
            return;
        }

        await studentAppNotifications.AnnouncementPublishedAsync(announcement, cancellationToken);
    }
}
