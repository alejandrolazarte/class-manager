namespace ClassManager.Subscriptions.Access;

public sealed record EffectiveFeature(string Code, int? Limit)
{
    public bool IsUnlimited => Limit is null;
}
