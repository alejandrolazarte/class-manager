using ClassManager.Subscriptions.Billing;

namespace ClassManager.Subscriptions.Stripe.Billing;

internal sealed class UnconfiguredBillingGateway : IBillingGateway
{
    private const string NotConfiguredMessage = "Stripe billing is not configured: set Stripe:SecretKey.";

    public Task<ExternalSubscription?> GetSubscriptionAsync(string externalSubscriptionId, CancellationToken cancellationToken) =>
        throw new InvalidOperationException(NotConfiguredMessage);

    public Task<string> CreateCustomerAsync(BillingCustomerRequest request, CancellationToken cancellationToken) =>
        throw new InvalidOperationException(NotConfiguredMessage);

    public Task<Uri> CreateCheckoutAsync(BillingCheckoutRequest request, CancellationToken cancellationToken) =>
        throw new InvalidOperationException(NotConfiguredMessage);

    public Task<Uri> CreatePortalSessionAsync(string externalCustomerId, Uri returnUrl, CancellationToken cancellationToken) =>
        throw new InvalidOperationException(NotConfiguredMessage);
}
