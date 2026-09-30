using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IMemberPushSubscriptionRepository
{
    void Add(MemberPushSubscription subscription);

    void Remove(MemberPushSubscription subscription);

    Task<MemberPushSubscription?> FindByEndpointAsync(string endpoint, CancellationToken cancellationToken);
}
