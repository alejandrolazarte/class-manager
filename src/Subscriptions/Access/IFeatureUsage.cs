namespace ClassManager.Subscriptions.Access;

public interface IFeatureUsage
{
    IReadOnlyCollection<string> CountedFeatureCodes { get; }

    Task<int> CountAsync(string featureCode, CancellationToken cancellationToken);
}
