using ClassManager.Infrastructure.Persistence;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Infrastructure.Subscriptions;

internal sealed class OrganizationSubscriberResolver(AppDbContext context, ITenantContext tenantContext) : ISubscriberResolver
{
    public async Task<Guid?> ResolveCurrentSubscriberIdAsync(CancellationToken cancellationToken)
    {
        var businessId = tenantContext.TenantId;
        return await context.Businesses
            .AsNoTracking()
            .Where(business => business.Id == businessId)
            .Select(business => (Guid?)business.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
