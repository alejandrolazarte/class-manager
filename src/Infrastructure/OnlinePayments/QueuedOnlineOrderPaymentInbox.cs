using ClassManager.Core.Abstractions.OnlinePayments;
using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.OnlinePayments;

internal sealed class QueuedOnlineOrderPaymentInbox(IBackgroundTaskOutbox outbox) : IOnlineOrderPaymentInbox
{
    public Task EnqueueAsync(OnlineCheckoutCompleted completedCheckout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(completedCheckout);
        return outbox.EnqueueAsync(
            new RecordOnlineOrderPaymentBackgroundTaskCommand(
                completedCheckout.BusinessId,
                completedCheckout.OrderId,
                completedCheckout.Amount,
                completedCheckout.ExternalPaymentId),
            cancellationToken);
    }
}
