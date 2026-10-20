using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Core.Brands;

public sealed record BrandRecord(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAt,
    IReadOnlyList<BrandOwner> Owners,
    IReadOnlyList<BrandBranch> Branches,
    IReadOnlyList<Subscription> Subscriptions,
    IReadOnlyList<SubscriptionFeature> CurrentSubscriptionFeatures,
    IReadOnlyList<PlanFeature> CurrentPlanFeatures)
{
    public Subscription? CurrentSubscription => Subscriptions.FirstOrDefault(subscription => !subscription.IsDeleted);
}
