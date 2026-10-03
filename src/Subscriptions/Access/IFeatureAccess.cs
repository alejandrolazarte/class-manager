namespace ClassManager.Subscriptions.Access;

public interface IFeatureAccess
{
    Task<EffectiveFeatures> GetCurrentAsync(CancellationToken cancellationToken);
}
