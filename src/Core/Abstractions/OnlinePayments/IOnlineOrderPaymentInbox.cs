namespace ClassManager.Core.Abstractions.OnlinePayments;

public interface IOnlineOrderPaymentInbox
{
    Task EnqueueAsync(OnlineCheckoutCompleted completedCheckout, CancellationToken cancellationToken);
}
