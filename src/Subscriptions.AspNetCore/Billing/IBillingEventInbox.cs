using ClassManager.Subscriptions.Billing;

namespace ClassManager.Subscriptions.AspNetCore.Billing;

public interface IBillingEventInbox
{
    Task ReceiveAsync(BillingEvent billingEvent, CancellationToken cancellationToken);
}
