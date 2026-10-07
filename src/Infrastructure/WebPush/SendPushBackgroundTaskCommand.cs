using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.WebPush;

public sealed record SendPushBackgroundTaskCommand(PushJob Push) : BackgroundTaskCommand(Push.TenantId);
