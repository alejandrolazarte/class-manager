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

    public const long SubscriberId = 41;
    public const long SubscriptionId = 42;
    public static readonly DateOnly Today = new(2026, 10, 3);
    public static readonly DateTimeOffset Now = new(2026, 10, 3, 12, 0, 0, TimeSpan.Zero);

    public static PlanFeature Included(string featureCode, int? limit = null) => PlanFeature.Create(PlanCode, featureCode, limit);

    public static SubscriptionFeature AddOn(
        string featureCode,
        int? limit = null,
        DateOnly? expiredOn = null,
        DateTimeOffset? createdOn = null) =>
        SubscriptionFeature.Create(SubscriptionId, featureCode, AddOnPrice, Currency, limit, expiredOn, createdOn ?? Now);

    public static EffectiveFeatures Combine(IEnumerable<PlanFeature> planFeatures, IEnumerable<SubscriptionFeature> subscriptionFeatures) =>
        EffectiveFeatures.Combine(PlanCode, planFeatures, subscriptionFeatures, Today);
}
