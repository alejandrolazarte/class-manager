using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Offers;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Core.Brands;

public sealed record BrandRecord(
    long Id,
    string Name,
    DateTimeOffset CreatedAt,
    IReadOnlyList<BrandOwner> Owners,
    IReadOnlyList<BrandBranch> Branches,
    IReadOnlyList<Subscription> Subscriptions,
    IReadOnlyList<SubscriptionFeature> CurrentSubscriptionFeatures,
    IReadOnlyList<PlanFeature> CurrentPlanFeatures,
    PlanOffer? CurrentOffer = null,
    string? ExternalCustomerId = null)
{
    public Subscription? CurrentSubscription => Subscriptions.FirstOrDefault(subscription => !subscription.IsDeleted);
}
