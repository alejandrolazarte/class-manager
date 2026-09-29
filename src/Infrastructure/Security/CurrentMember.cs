using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.Security;

internal sealed class CurrentMember(AppDbContext context, ICurrentUser currentUser, ITenantContext tenantContext) : ICurrentMember
{
    private MemberAccess? _resolvedAccess;
    private bool _isResolved;

    public async Task<MemberAccess?> GetAccessAsync(CancellationToken cancellationToken)
    {
        if (!_isResolved)
        {
            _resolvedAccess = await ResolveAsync(cancellationToken);
            _isResolved = true;
        }

        return _resolvedAccess;
    }

    private async Task<MemberAccess?> ResolveAsync(CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return null;
        }

        var businessId = tenantContext.TenantId;
        var branchMember = await context.BusinessMembers
            .AsNoTracking()
            .Where(member => member.UserId == userId)
            .Select(member => new { member.Role, member.InstructorId })
            .FirstOrDefaultAsync(cancellationToken);
        var isBrandOwner = await context.Businesses
            .AsNoTracking()
            .Where(business => business.Id == businessId)
            .Join(
                context.OrganizationMembers,
                business => business.OrganizationId,
                member => member.OrganizationId,
                (_, member) => member)
            .AnyAsync(member => member.UserId == userId && member.Role == OrganizationRole.BrandOwner, cancellationToken);

        if (branchMember is null && !isBrandOwner)
        {
            return null;
        }

        var permissions = isBrandOwner ? SystemRolePermissions.BrandOwner : SystemRolePermissions.Of(branchMember!.Role);
        return new MemberAccess(userId, businessId, branchMember?.Role, branchMember?.InstructorId, isBrandOwner, permissions);
    }
}
