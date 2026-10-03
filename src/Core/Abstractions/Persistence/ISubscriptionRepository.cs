using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Core.Abstractions.Persistence;

public interface ISubscriptionRepository
{
    Task<Plan> GetSignUpPlanAsync(CancellationToken cancellationToken);

    void Add(Subscription subscription);

    Task<IReadOnlyList<Plan>> ListActivePlansAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<PlanFeature>> ListPlanFeaturesAsync(CancellationToken cancellationToken);

    Task<Subscription?> GetCurrentAsync(Guid subscriberId, CancellationToken cancellationToken);
}
