using ClassManager.Notifications.WebPush;
using Microsoft.Extensions.Options;

namespace ClassManager.Infrastructure.WebPush;

internal sealed class PushPublisher(ITenantContext tenantContext, PushOutbox queue, IOptions<VapidOptions> options)
{
    public void PublishToStudents(IReadOnlyCollection<Guid>? clientIds, PushMessage message)
    {
        if (!options.Value.IsConfigured || clientIds?.Count == 0)
        {
            return;
        }

        queue.Enqueue(new StudentAppPush(tenantContext.TenantId, clientIds, message));
    }

    public void PublishToMembers(IReadOnlyCollection<Guid> userIds, PushMessage message)
    {
        if (!options.Value.IsConfigured || userIds.Count == 0)
        {
            return;
        }

        queue.Enqueue(new TeamPush(tenantContext.TenantId, userIds, message));
    }
}
