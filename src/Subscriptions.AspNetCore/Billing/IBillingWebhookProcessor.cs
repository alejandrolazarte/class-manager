using ClassManager.Subscriptions.Billing;

namespace ClassManager.Subscriptions.AspNetCore.Billing;

public interface IBillingWebhookProcessor
{
    Task ProcessAsync(BillingEvent billingEvent, CancellationToken cancellationToken);
}
