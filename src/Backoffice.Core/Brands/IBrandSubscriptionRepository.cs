using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Core.Brands;

public interface IBrandSubscriptionRepository
{
    Task<bool> BrandExistsAsync(long brandId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Plan>> ListActivePlansAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Feature>> ListFeaturesAsync(CancellationToken cancellationToken);

    Task<Subscription?> FindCurrentSubscriptionAsync(long brandId, CancellationToken cancellationToken);

    Task<IReadOnlyList<SubscriptionFeature>> ListAddOnsIncludingRemovedAsync(long subscriptionId, CancellationToken cancellationToken);

    void Add(Subscription subscription);

    void Add(SubscriptionFeature addOn);
}
