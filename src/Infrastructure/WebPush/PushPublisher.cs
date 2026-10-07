using ClassManager.Infrastructure.BackgroundTasks;
using ClassManager.Notifications.WebPush;
using Microsoft.Extensions.Options;

namespace ClassManager.Infrastructure.WebPush;

internal sealed class PushPublisher(ITenantContext tenantContext, IBackgroundTaskOutbox queue, IOptions<VapidOptions> options)
{
    public async Task PublishToStudentsAsync(IReadOnlyCollection<Guid>? clientIds, PushMessage message, CancellationToken cancellationToken)
    {
        if (!options.Value.IsConfigured || clientIds?.Count == 0)
        {
            return;
        }

        await queue.EnqueueAsync(new SendPushTask(new StudentAppPush(tenantContext.TenantId, clientIds, message)), cancellationToken);
    }

    public async Task PublishToMembersAsync(IReadOnlyCollection<Guid> userIds, PushMessage message, CancellationToken cancellationToken)
    {
        if (!options.Value.IsConfigured || userIds.Count == 0)
        {
            return;
        }

        await queue.EnqueueAsync(new SendPushTask(new TeamPush(tenantContext.TenantId, userIds, message)), cancellationToken);
    }
}
