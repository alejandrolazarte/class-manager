using System.Collections.Concurrent;
using ClassManager.Subscriptions.Billing;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed class FakeBillingGateway : IBillingGateway
{
    private readonly ConcurrentDictionary<string, ExternalSubscription> _subscriptions = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, int> _readCounts = new(StringComparer.Ordinal);

    public void Set(ExternalSubscription subscription) => _subscriptions[subscription.Id] = subscription;

    public int ReadCount(string externalSubscriptionId) => _readCounts.GetValueOrDefault(externalSubscriptionId);

    public Task<ExternalSubscription?> GetSubscriptionAsync(string externalSubscriptionId, CancellationToken cancellationToken)
    {
        _readCounts.AddOrUpdate(externalSubscriptionId, 1, (_, count) => count + 1);
        return Task.FromResult(_subscriptions.GetValueOrDefault(externalSubscriptionId));
    }
}
