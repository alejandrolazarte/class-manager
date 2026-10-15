using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.Notifications;

public sealed record PublishScheduledAnnouncementBackgroundTaskCommand(Guid BusinessId, Guid AnnouncementId)
    : BackgroundTaskCommand(BusinessId);
