using ClassManager.Infrastructure.BackgroundTasks;
using ClassManager.Infrastructure.Notifications;
using ClassManager.Notifications.WebPush;
using Microsoft.Extensions.Options;

namespace ClassManager.Infrastructure.WebPush;

internal sealed class PushPublisher(ITenantContext tenantContext, IBackgroundTaskOutbox queue, IOptions<VapidOptions> options)
{
    public async Task PublishToStudentsAsync(IReadOnlyCollection<long>? clientIds, LocalizedPushMessage message, CancellationToken cancellationToken)
    {
        if (!options.Value.IsConfigured || clientIds?.Count == 0)
        {
            return;
        }

        await EnqueueAsync(new StudentAppPush(tenantContext.TenantId, clientIds, message.Default) { MessagesByLanguage = message.ByLanguage }, cancellationToken);
    }

    public async Task PublishToStudentAccountsAsync(
        long clientId,
        IReadOnlyCollection<Guid> userIds,
        LocalizedPushMessage message,
        CancellationToken cancellationToken)
    {
        if (!options.Value.IsConfigured || userIds.Count == 0)
        {
            return;
        }

        await EnqueueAsync(
            new StudentAppPush(tenantContext.TenantId, [clientId], message.Default, userIds) { MessagesByLanguage = message.ByLanguage },
            cancellationToken);
    }

    public async Task PublishToMembersAsync(IReadOnlyCollection<Guid> userIds, LocalizedPushMessage message, CancellationToken cancellationToken)
    {
        if (!options.Value.IsConfigured || userIds.Count == 0)
        {
            return;
        }

        await EnqueueAsync(new TeamPush(tenantContext.TenantId, userIds, message.Default) { MessagesByLanguage = message.ByLanguage }, cancellationToken);
    }

    private Task EnqueueAsync(PushJob push, CancellationToken cancellationToken) =>
        queue.EnqueueAsync(new SendPushBackgroundTaskCommand(push), cancellationToken);
}
