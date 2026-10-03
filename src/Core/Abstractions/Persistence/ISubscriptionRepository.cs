using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Core.Abstractions.Persistence;

public interface ISubscriptionRepository
{
    Task<Plan> GetDefaultPlanAsync(CancellationToken cancellationToken);

    void Add(Subscription subscription);
}
