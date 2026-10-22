using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.Notifications;

public sealed record PublishScheduledAnnouncementBackgroundTaskCommand(long BusinessId, long AnnouncementId)
    : BackgroundTaskCommand(BusinessId);
