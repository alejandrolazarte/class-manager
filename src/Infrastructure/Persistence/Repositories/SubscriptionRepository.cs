using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class SubscriptionRepository(AppDbContext context) : ISubscriptionRepository
{
    public Task<Plan> GetDefaultPlanAsync(CancellationToken cancellationToken) =>
        context.Plans.AsNoTracking().SingleAsync(plan => plan.IsDefault, cancellationToken);

    public void Add(Subscription subscription) => context.Subscriptions.Add(subscription);
}
