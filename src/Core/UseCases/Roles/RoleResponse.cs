using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Roles;

namespace ClassManager.Core.UseCases.Roles;

public sealed record RoleResponse(
    Guid? Id,
    BusinessRole? SystemRole,
    string? Name,
    IReadOnlyList<string> Permissions,
    string? CopiedFrom,
    int MemberCount)
{
    public static RoleResponse From(BusinessRole systemRole, int memberCount) =>
        new(null, systemRole, null, [.. SystemRolePermissions.Of(systemRole).Order(StringComparer.Ordinal)], null, memberCount);

    public static RoleResponse From(CustomRole role, int memberCount) =>
        new(role.Id, null, role.Name, role.Permissions, role.CopiedFrom, memberCount);
}

public sealed record DeletedRoleResponse(Guid Id);
