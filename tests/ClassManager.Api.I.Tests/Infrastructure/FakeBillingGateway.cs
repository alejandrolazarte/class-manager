using System.Collections.Concurrent;
using ClassManager.Subscriptions.Billing;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed class FakeBillingGateway : IBillingGateway
{
    public const string CheckoutHost = "https://checkout.stripe.test/";
    public const string PortalHost = "https://billing.stripe.test/";

    private const string CustomerIdPrefix = "cus_";

    private readonly ConcurrentDictionary<string, ExternalSubscription> _subscriptions = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, int> _readCounts = new(StringComparer.Ordinal);
    private readonly ConcurrentQueue<BillingCustomerRequest> _customerRequests = new();
    private readonly ConcurrentQueue<BillingCheckoutRequest> _checkoutRequests = new();

    public void Set(ExternalSubscription subscription) => _subscriptions[subscription.Id] = subscription;

    public int ReadCount(string externalSubscriptionId) => _readCounts.GetValueOrDefault(externalSubscriptionId);

    public IReadOnlyList<BillingCustomerRequest> CustomersCreatedFor(long subscriberId) =>
        [.. _customerRequests.Where(request => request.SubscriberId == subscriberId)];

    public IReadOnlyList<BillingCheckoutRequest> CheckoutsFor(long subscriberId) =>
        [.. _checkoutRequests.Where(request => request.SubscriberId == subscriberId)];

    public Task<ExternalSubscription?> GetSubscriptionAsync(string externalSubscriptionId, CancellationToken cancellationToken)
    {
        _readCounts.AddOrUpdate(externalSubscriptionId, 1, (_, count) => count + 1);
        return Task.FromResult(_subscriptions.GetValueOrDefault(externalSubscriptionId));
    }

    public Task<string> CreateCustomerAsync(BillingCustomerRequest request, CancellationToken cancellationToken)
    {
        _customerRequests.Enqueue(request);
        return Task.FromResult(CustomerIdPrefix + Guid.NewGuid().ToString("N"));
    }

    public Task<Uri> CreateCheckoutAsync(BillingCheckoutRequest request, CancellationToken cancellationToken)
    {
        _checkoutRequests.Enqueue(request);
        return Task.FromResult(new Uri(CheckoutHost + request.ExternalCustomerId));
    }

    public Task<Uri> CreatePortalSessionAsync(string externalCustomerId, Uri returnUrl, CancellationToken cancellationToken) =>
        Task.FromResult(new Uri(PortalHost + externalCustomerId));
}
