namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class PushSubscriptionRepository(AppDbContext context) : IPushSubscriptionRepository
{
    public void Add(PushSubscription subscription) => context.PushSubscriptions.Add(subscription);

    public void Remove(PushSubscription subscription) => context.PushSubscriptions.Remove(subscription);

    public Task<PushSubscription?> FindByEndpointAsync(string endpoint, CancellationToken cancellationToken) =>
        context.PushSubscriptions.FirstOrDefaultAsync(subscription => subscription.Endpoint == endpoint, cancellationToken);
}
