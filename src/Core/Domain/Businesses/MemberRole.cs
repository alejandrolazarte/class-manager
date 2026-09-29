using System.Collections.Frozen;

using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Roles;

namespace ClassManager.Core.Domain.Businesses;

public sealed record MemberRole(BusinessRole Role, Guid? CustomRoleId, IReadOnlySet<string> Permissions)
{
    public static MemberRole System(BusinessRole role) => new(role, null, SystemRolePermissions.Of(role));

    public static MemberRole Custom(CustomRole role) =>
        new(BusinessRole.Custom, role.Id, role.Permissions.Where(Authorization.Permissions.Assignable.Contains).ToFrozenSet(StringComparer.Ordinal));

    public bool NeedsInstructor => SystemRolePermissions.NeedsInstructor(Permissions);
}
