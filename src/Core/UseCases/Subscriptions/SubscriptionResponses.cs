using ClassManager.Subscriptions.Access;
using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Core.UseCases.Subscriptions;

public sealed record FeatureLimitResponse(string Code, int? Limit, int? Used = null);

public sealed record CurrentSubscriptionResponse(
    string PlanCode,
    bool IsActive,
    DateOnly? ExpiredOn,
    IReadOnlyList<FeatureLimitResponse> Features)
{
    public static CurrentSubscriptionResponse From(EffectiveFeatures features, IReadOnlyDictionary<string, int>? usageByFeatureCode = null) =>
        new(
            features.PlanCode,
            features.IsActive,
            features.ExpiredOn,
            [.. features.All
                .OrderBy(feature => feature.Code, StringComparer.Ordinal)
                .Select(feature => new FeatureLimitResponse(
                    feature.Code,
                    feature.Limit,
                    usageByFeatureCode is not null && usageByFeatureCode.TryGetValue(feature.Code, out var used) ? used : null))]);
}

public sealed record PlanResponse(
    string Code,
    decimal? ListPrice,
    string Currency,
    BillingPeriod BillingPeriod,
    int? DurationInDays,
    IReadOnlyList<FeatureLimitResponse> Features);

public sealed record OrganizationSubscriptionResponse(
    string PlanCode,
    decimal Price,
    string Currency,
    DateTimeOffset CreatedOn,
    DateOnly? ExpiredOn,
    bool IsActive);
