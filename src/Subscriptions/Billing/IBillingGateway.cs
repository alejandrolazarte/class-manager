namespace ClassManager.Subscriptions.Billing;

public interface IBillingGateway
{
    Task<ExternalSubscription?> GetSubscriptionAsync(string externalSubscriptionId, CancellationToken cancellationToken);
}
