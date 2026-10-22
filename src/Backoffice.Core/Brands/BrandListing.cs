using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Core.Brands;

public sealed record BrandListing(
    long Id,
    string Name,
    DateTimeOffset CreatedAt,
    BrandOwner? Owner,
    Subscription? CurrentSubscription,
    int BranchCount,
    int StudentCount);
