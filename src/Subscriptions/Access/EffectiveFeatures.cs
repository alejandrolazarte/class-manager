using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Subscriptions.Access;

public sealed class EffectiveFeatures
{
    private readonly Dictionary<string, EffectiveFeature> _featuresByCode;

    private EffectiveFeatures(string planCode, Dictionary<string, EffectiveFeature> featuresByCode)
    {
        PlanCode = planCode;
        _featuresByCode = featuresByCode;
    }

    public string PlanCode { get; }

    public IReadOnlyCollection<EffectiveFeature> All => _featuresByCode.Values;

    public static EffectiveFeatures Combine(
        string planCode,
        IEnumerable<PlanFeature> planFeatures,
        IEnumerable<SubscriptionFeature> subscriptionFeatures,
        DateOnly today)
    {
        ArgumentNullException.ThrowIfNull(planFeatures);
        ArgumentNullException.ThrowIfNull(subscriptionFeatures);

        var featuresByCode = new Dictionary<string, EffectiveFeature>(StringComparer.Ordinal);
        var grants = planFeatures
            .Select(planFeature => new EffectiveFeature(planFeature.FeatureCode, planFeature.Limit))
            .Concat(subscriptionFeatures
                .Where(subscriptionFeature => subscriptionFeature.IsActiveOn(today))
                .Select(subscriptionFeature => new EffectiveFeature(subscriptionFeature.FeatureCode, subscriptionFeature.Limit)));

        foreach (var grant in grants)
        {
            featuresByCode[grant.Code] = featuresByCode.TryGetValue(grant.Code, out var existing) ? Larger(existing, grant) : grant;
        }

        return new EffectiveFeatures(planCode, featuresByCode);
    }

    public bool Has(string featureCode) => _featuresByCode.ContainsKey(featureCode);

    public int? LimitOf(string featureCode) =>
        _featuresByCode.TryGetValue(featureCode, out var feature) ? feature.Limit : 0;

    public bool AllowsAnother(string featureCode, int currentCount) =>
        _featuresByCode.TryGetValue(featureCode, out var feature) && (feature.IsUnlimited || currentCount < feature.Limit);

    private static EffectiveFeature Larger(EffectiveFeature first, EffectiveFeature second) =>
        first.IsUnlimited || second.IsUnlimited
            ? first with { Limit = null }
            : first with { Limit = Math.Max(first.Limit!.Value, second.Limit!.Value) };
}
