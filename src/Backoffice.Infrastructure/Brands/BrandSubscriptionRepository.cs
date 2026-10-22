using ClassManager.Backoffice.Core.Brands;
using ClassManager.Infrastructure.Persistence;
using ClassManager.Records;
using ClassManager.Subscriptions.Catalog;
using ClassManager.Subscriptions.Subscribers;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Backoffice.Infrastructure.Brands;

internal sealed class BrandSubscriptionRepository(AppDbContext context) : IBrandSubscriptionRepository
{
    public Task<bool> BrandExistsAsync(long brandId, CancellationToken cancellationToken) =>
        context.Organizations.AnyAsync(organization => organization.Id == brandId, cancellationToken);

    public async Task<IReadOnlyList<Plan>> ListActivePlansAsync(CancellationToken cancellationToken) =>
        await context.Plans.AsNoTracking().Where(plan => plan.IsActive).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Feature>> ListFeaturesAsync(CancellationToken cancellationToken) =>
        await context.Features.AsNoTracking().ToListAsync(cancellationToken);

    public Task<Subscription?> FindCurrentSubscriptionAsync(long brandId, CancellationToken cancellationToken) =>
        context.Subscriptions.WhereNotDeleted().FirstOrDefaultAsync(subscription => subscription.SubscriberId == brandId, cancellationToken);

    public async Task<IReadOnlyList<SubscriptionFeature>> ListAddOnsIncludingRemovedAsync(long subscriptionId, CancellationToken cancellationToken) =>
        await context.SubscriptionFeatures.IncludeDeleted()
            .Where(addOn => addOn.SubscriptionId == subscriptionId)
            .ToListAsync(cancellationToken);

    public void Add(Subscription subscription) => context.Subscriptions.Add(subscription);

    public void Add(SubscriptionFeature addOn) => context.SubscriptionFeatures.Add(addOn);
}
