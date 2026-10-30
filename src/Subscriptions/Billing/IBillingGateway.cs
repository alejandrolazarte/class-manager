namespace ClassManager.Subscriptions.Billing;

public interface IBillingGateway
{
    Task<ExternalSubscription?> GetSubscriptionAsync(string externalSubscriptionId, CancellationToken cancellationToken);

    Task<string> CreateCustomerAsync(BillingCustomerRequest request, CancellationToken cancellationToken);

    Task<Uri> CreateCheckoutAsync(BillingCheckoutRequest request, CancellationToken cancellationToken);

    Task<Uri> CreatePortalSessionAsync(BillingPortalRequest request, CancellationToken cancellationToken);
}
