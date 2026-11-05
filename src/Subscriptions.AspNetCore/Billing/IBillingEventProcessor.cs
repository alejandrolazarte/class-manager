using ClassManager.Subscriptions.Billing;

namespace ClassManager.Subscriptions.AspNetCore.Billing;

public interface IBillingEventProcessor
{
    Task ProcessAsync(BillingEvent billingEvent, CancellationToken cancellationToken);
}
