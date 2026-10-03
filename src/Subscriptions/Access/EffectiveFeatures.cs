using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Subscriptions.Access;

public sealed class EffectiveFeatures
{
    private readonly Dictionary<string, EffectiveFeature> _featuresByCode;

    private EffectiveFeatures(string planCode, bool isActive, DateOnly? endsOn, Dictionary<string, EffectiveFeature> featuresByCode)
    {
        PlanCode = planCode;
        IsActive = isActive;
        EndsOn = endsOn;
        _featuresByCode = featuresByCode;
    }

    public string PlanCode { get; }

    public bool IsActive { get; }

    public DateOnly? EndsOn { get; }

    public IReadOnlyCollection<EffectiveFeature> All => _featuresByCode.Values;

    public static EffectiveFeatures Combine(
        string planCode,
        IEnumerable<PlanFeature> planFeatures,
        IEnumerable<SubscriptionFeature> subscriptionFeatures,
        DateOnly today,
        DateOnly? endsOn = null)
    {
        ArgumentNullException.ThrowIfNull(planFeatures);
        ArgumentNullException.ThrowIfNull(subscriptionFeatures);

        var featuresByCode = planFeatures.ToDictionary(
            planFeature => planFeature.FeatureCode,
            planFeature => new EffectiveFeature(planFeature.FeatureCode, planFeature.Limit),
            StringComparer.Ordinal);
        var overridesInOrder = subscriptionFeatures
            .Where(subscriptionFeature => subscriptionFeature.IsActiveOn(today))
            .OrderBy(subscriptionFeature => subscriptionFeature.StartsOn)
            .ThenBy(subscriptionFeature => subscriptionFeature.CreatedAt);

        foreach (var subscriptionFeature in overridesInOrder)
        {
            featuresByCode[subscriptionFeature.FeatureCode] = new EffectiveFeature(subscriptionFeature.FeatureCode, subscriptionFeature.Limit);
        }

        return new EffectiveFeatures(planCode, isActive: true, endsOn, featuresByCode);
    }

    public static EffectiveFeatures Inactive(string lastPlanCode, DateOnly? endedOn = null) =>
        new(lastPlanCode, isActive: false, endedOn, new Dictionary<string, EffectiveFeature>(StringComparer.Ordinal));

    public bool Has(string featureCode) => _featuresByCode.ContainsKey(featureCode);

    public int? LimitOf(string featureCode) =>
        _featuresByCode.TryGetValue(featureCode, out var feature) ? feature.Limit : 0;

    public bool AllowsAnother(string featureCode, int currentCount) =>
        _featuresByCode.TryGetValue(featureCode, out var feature) && (feature.IsUnlimited || currentCount < feature.Limit);
}
