namespace ClassManager.Subscriptions.Catalog;

public sealed class PlanFeature
{
    private PlanFeature()
    {
    }

    public string PlanCode { get; private set; } = string.Empty;
    public string FeatureCode { get; private set; } = string.Empty;
    public int? Limit { get; private set; }

    public static PlanFeature Create(string planCode, string featureCode, int? limit = null) =>
        new()
        {
            PlanCode = CatalogRules.RequireCode(planCode, nameof(planCode)),
            FeatureCode = CatalogRules.RequireCode(featureCode, nameof(featureCode)),
            Limit = CatalogRules.RequireLimit(limit, nameof(limit)),
        };
}
