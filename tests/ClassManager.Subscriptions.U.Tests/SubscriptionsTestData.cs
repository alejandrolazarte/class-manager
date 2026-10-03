namespace ClassManager.Subscriptions.U.Tests;

internal static class SubscriptionsTestData
{
    public const string PlanCode = "pro";
    public const string IncludedFeatureCode = "shop";
    public const string MissingFeatureCode = "custom-roles";
    public const string AddOnFeatureCode = "import-export";
    public const string CountedFeatureCode = "branches";
    public const string Currency = "USD";
    public const int PlanLimit = 3;
    public const decimal AddOnPrice = 5m;

    public static readonly Guid SubscriberId = Guid.Parse("8f5d3c1e-0b1a-4c55-9a1e-3f2d6c7b8a90");
    public static readonly Guid SubscriptionId = Guid.Parse("2a7e9b4c-6d3f-4e18-8c2b-1f0a9d5e7c63");
    public static readonly DateOnly Today = new(2026, 10, 3);
    public static readonly DateTimeOffset Now = new(2026, 10, 3, 12, 0, 0, TimeSpan.Zero);

    public static PlanFeature Included(string featureCode, int? limit = null) => PlanFeature.Create(PlanCode, featureCode, limit);

    public static SubscriptionFeature AddOn(
        string featureCode,
        DateOnly startsOn,
        DateOnly? endsOn = null,
        int? limit = null,
        DateTimeOffset? createdAt = null) =>
        SubscriptionFeature.Create(SubscriptionId, featureCode, AddOnPrice, Currency, limit, startsOn, endsOn, createdAt ?? Now);

    public static EffectiveFeatures Combine(IEnumerable<PlanFeature> planFeatures, IEnumerable<SubscriptionFeature> subscriptionFeatures) =>
        EffectiveFeatures.Combine(PlanCode, planFeatures, subscriptionFeatures, Today);
}
