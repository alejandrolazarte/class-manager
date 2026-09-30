using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IPushSubscriptionRepository
{
    void Add(PushSubscription subscription);

    void Remove(PushSubscription subscription);

    Task<PushSubscription?> FindByEndpointAsync(string endpoint, CancellationToken cancellationToken);
}
