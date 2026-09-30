namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class MemberPushSubscriptionRepository(AppDbContext context) : IMemberPushSubscriptionRepository
{
    public void Add(MemberPushSubscription subscription) => context.MemberPushSubscriptions.Add(subscription);

    public void Remove(MemberPushSubscription subscription) => context.MemberPushSubscriptions.Remove(subscription);

    public Task<MemberPushSubscription?> FindByEndpointAsync(string endpoint, CancellationToken cancellationToken) =>
        context.MemberPushSubscriptions.FirstOrDefaultAsync(subscription => subscription.Endpoint == endpoint, cancellationToken);
}
