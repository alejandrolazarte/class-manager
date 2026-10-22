namespace ClassManager.Subscriptions.Access;

public interface ISubscriberResolver
{
    Task<long?> ResolveCurrentSubscriberIdAsync(CancellationToken cancellationToken);
}
