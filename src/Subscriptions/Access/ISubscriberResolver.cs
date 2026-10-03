namespace ClassManager.Subscriptions.Access;

public interface ISubscriberResolver
{
    Task<Guid?> ResolveCurrentSubscriberIdAsync(CancellationToken cancellationToken);
}
