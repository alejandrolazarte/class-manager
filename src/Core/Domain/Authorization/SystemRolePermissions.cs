using System.Collections.Frozen;

using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Domain.Authorization;

public static class SystemRolePermissions
{
    private static readonly FrozenSet<string> EveryPermission = Permissions.All.ToFrozenSet(StringComparer.Ordinal);

    public static IReadOnlySet<string> BrandOwner => EveryPermission;

    public static IReadOnlySet<string> Of(BusinessRole role) =>
        role switch
        {
            BusinessRole.BranchOwner => EveryPermission,
            _ => FrozenSet<string>.Empty,
        };
}
