using ClassManager.Core.Abstractions.Security;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.Security;

internal sealed class BranchDirectory(AppDbContext context) : IBranchDirectory
{
    public async Task<BranchAccess?> FindDefaultAsync(Guid userId, CancellationToken cancellationToken)
    {
        var firstMembership = await context.BusinessMembers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(member => member.UserId == userId)
            .OrderBy(member => member.Id)
            .Select(member => (Guid?)member.TenantId)
            .FirstOrDefaultAsync(cancellationToken);
        if (firstMembership is { } businessId)
        {
            return await FindAsync(userId, businessId, cancellationToken);
        }

        var firstBrandBusiness = await context.Businesses
            .AsNoTracking()
            .Where(business => BrandOrganizationIds(userId).Contains(business.OrganizationId))
            .OrderBy(business => business.CreatedAt)
            .Select(business => (Guid?)business.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return firstBrandBusiness is { } brandBusinessId ? await FindAsync(userId, brandBusinessId, cancellationToken) : null;
    }

    public async Task<BranchAccess?> FindAsync(Guid userId, Guid businessId, CancellationToken cancellationToken)
    {
        var branches = await ListWhereAsync(userId, businessId, cancellationToken);
        return branches.Count > 0 ? branches[0] : null;
    }

    public Task<IReadOnlyList<BranchAccess>> ListAsync(Guid userId, CancellationToken cancellationToken) =>
        ListWhereAsync(userId, null, cancellationToken);

    private async Task<IReadOnlyList<BranchAccess>> ListWhereAsync(Guid userId, Guid? businessId, CancellationToken cancellationToken)
    {
        var memberships = await context.BusinessMembers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(member => member.UserId == userId && (businessId == null || member.TenantId == businessId))
            .Select(member => new { member.TenantId, member.Role })
            .ToListAsync(cancellationToken);
        var brandOrganizationIds = await BrandOrganizationIds(userId).ToListAsync(cancellationToken);
        var memberBusinessIds = memberships.Select(membership => membership.TenantId).ToList();
        var businesses = await context.Businesses
            .AsNoTracking()
            .Where(business => businessId == null || business.Id == businessId)
            .Where(business => memberBusinessIds.Contains(business.Id) || brandOrganizationIds.Contains(business.OrganizationId))
            .Select(business => new { business.Id, business.Name, business.OrganizationId })
            .ToListAsync(cancellationToken);
        var rolesByBusinessId = memberships.ToDictionary(membership => membership.TenantId, membership => membership.Role);

        return
        [
            .. businesses.Select(business => new BranchAccess(
                business.Id,
                business.Name,
                business.OrganizationId,
                rolesByBusinessId.TryGetValue(business.Id, out var role) ? role : null,
                brandOrganizationIds.Contains(business.OrganizationId))),
        ];
    }

    private IQueryable<Guid> BrandOrganizationIds(Guid userId) =>
        context.OrganizationMembers
            .Where(member => member.UserId == userId && member.Role == OrganizationRole.BrandOwner)
            .Select(member => member.OrganizationId);
}
