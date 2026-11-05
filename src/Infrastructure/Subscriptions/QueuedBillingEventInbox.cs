using ClassManager.Infrastructure.BackgroundTasks;
using ClassManager.Subscriptions.AspNetCore.Billing;
using ClassManager.Subscriptions.Billing;

namespace ClassManager.Infrastructure.Subscriptions;

internal sealed class QueuedBillingEventInbox(IBackgroundTaskOutbox outbox) : IBillingEventInbox
{
    public Task ReceiveAsync(BillingEvent billingEvent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(billingEvent);
        return outbox.EnqueueAsync(
            new ProcessBillingEventBackgroundTaskCommand(billingEvent.Id, billingEvent.ExternalSubscriptionId),
            cancellationToken);
    }
}
