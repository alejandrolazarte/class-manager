using ClassManager.Core.Domain.Authorization;
using ClassManager.Infrastructure.Persistence;

namespace ClassManager.Infrastructure.Notifications;

internal sealed class TeamRecipients(AppDbContext context)
{
    public async Task<IReadOnlyCollection<Guid>> WithPermissionAsync(string permission, CancellationToken cancellationToken)
    {
        var members = await context.BusinessMembers.AsNoTracking().Include(member => member.User).ToListAsync(cancellationToken);
        var customRoles = await context.CustomRoles.AsNoTracking().ToDictionaryAsync(role => role.Id, cancellationToken);
        var userIds = members
            .Where(member => PermissionsOf(member, customRoles).Contains(permission))
            .Where(member => member.User.AccountId != null)
            .Select(member => member.User.AccountId!.Value)
            .ToHashSet();
        var organizationId = await context.Businesses.AsNoTracking()
            .Where(business => business.Id == context.CurrentTenantId)
            .Select(business => (long?)business.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);
        userIds.UnionWith(await context.OrganizationMembers.AsNoTracking()
            .Where(member => member.OrganizationId == organizationId && member.Role == OrganizationRole.BrandOwner)
            .Select(member => member.UserId)
            .ToListAsync(cancellationToken));
        return userIds;
    }

    private static IReadOnlySet<string> PermissionsOf(BusinessMember member, Dictionary<long, CustomRole> customRoles) =>
        member.CustomRoleId is { } customRoleId
            ? customRoles.TryGetValue(customRoleId, out var customRole) ? MemberRole.Custom(customRole).Permissions : new HashSet<string>()
            : SystemRolePermissions.Of(member.Role);
}
