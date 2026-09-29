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
            .Select(member => new { member.TenantId, member.Role, member.CustomRoleId })
            .ToListAsync(cancellationToken);
        var customRoleIds = memberships
            .Select(membership => membership.CustomRoleId)
            .OfType<Guid>()
            .ToList();
        var customRoleNames = await context.CustomRoles
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(role => customRoleIds.Contains(role.Id))
            .ToDictionaryAsync(role => role.Id, role => role.Name, cancellationToken);
        var brandOrganizationIds = await BrandOrganizationIds(userId).ToListAsync(cancellationToken);
        var memberBusinessIds = memberships.Select(membership => membership.TenantId).ToList();
        var businesses = await context.Businesses
            .AsNoTracking()
            .Where(business => businessId == null || business.Id == businessId)
            .Where(business => memberBusinessIds.Contains(business.Id) || brandOrganizationIds.Contains(business.OrganizationId))
            .Select(business => new { business.Id, business.Name, business.OrganizationId })
            .ToListAsync(cancellationToken);
        var membershipsByBusinessId = memberships.ToDictionary(membership => membership.TenantId);

        return
        [
            .. businesses.Select(business =>
            {
                var membership = membershipsByBusinessId.GetValueOrDefault(business.Id);
                return new BranchAccess(
                    business.Id,
                    business.Name,
                    business.OrganizationId,
                    membership?.Role,
                    brandOrganizationIds.Contains(business.OrganizationId),
                    membership?.CustomRoleId is { } customRoleId ? customRoleNames.GetValueOrDefault(customRoleId) : null);
            }),
        ];
    }

    private IQueryable<Guid> BrandOrganizationIds(Guid userId) =>
        context.OrganizationMembers
            .Where(member => member.UserId == userId && member.Role == OrganizationRole.BrandOwner)
            .Select(member => member.OrganizationId);
}
