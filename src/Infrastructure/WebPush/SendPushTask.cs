using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.WebPush;

public sealed record SendPushTask(PushJob Push) : BackgroundTaskCommand(Push.TenantId)
{
    public const string TypeName = "push.send";
}
