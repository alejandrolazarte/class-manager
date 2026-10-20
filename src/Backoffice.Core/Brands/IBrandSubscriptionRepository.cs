using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Core.Brands;

public interface IBrandSubscriptionRepository
{
    Task<bool> BrandExistsAsync(Guid brandId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Plan>> ListActivePlansAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Feature>> ListFeaturesAsync(CancellationToken cancellationToken);

    Task<Subscription?> FindCurrentSubscriptionAsync(Guid brandId, CancellationToken cancellationToken);

    Task<IReadOnlyList<SubscriptionFeature>> ListAddOnsIncludingRemovedAsync(Guid subscriptionId, CancellationToken cancellationToken);

    void Add(Subscription subscription);

    void Add(SubscriptionFeature addOn);
}
