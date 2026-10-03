namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class BusinessRepository(AppDbContext context, ITenantContext tenantContext) : IBusinessRepository
{
    public void Add(Business business) => context.Businesses.Add(business);

    public Task<Business?> GetCurrentAsync(CancellationToken cancellationToken) =>
        context.Businesses.AsNoTracking().FirstOrDefaultAsync(business => business.Id == tenantContext.TenantId, cancellationToken);

    public Task<Business?> GetCurrentForUpdateAsync(CancellationToken cancellationToken) =>
        context.Businesses.FirstOrDefaultAsync(business => business.Id == tenantContext.TenantId, cancellationToken);

    public Task<bool> IsSlugTakenAsync(string slug, CancellationToken cancellationToken) =>
        context.Businesses.AnyAsync(business => business.Slug == slug, cancellationToken);

    public Task<int> CountInOrganizationAsync(Guid organizationId, CancellationToken cancellationToken) =>
        context.Businesses.CountAsync(business => business.OrganizationId == organizationId, cancellationToken);
}
