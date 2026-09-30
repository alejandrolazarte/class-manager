using Microsoft.Extensions.Options;

namespace ClassManager.Infrastructure.WebPush;

internal sealed class FamilyPushPublisher(ITenantContext tenantContext, FamilyPushOutbox queue, IOptions<VapidOptions> options)
{
    public void Publish(IReadOnlyCollection<Guid>? clientIds, FamilyPushMessage message)
    {
        if (!options.Value.IsConfigured || clientIds?.Count == 0)
        {
            return;
        }

        queue.Enqueue(new FamilyPush(tenantContext.TenantId, clientIds, message));
    }
}
