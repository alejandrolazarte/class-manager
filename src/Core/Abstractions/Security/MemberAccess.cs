using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Abstractions.Security;

public sealed record MemberAccess(
    Guid UserId,
    Guid BusinessId,
    BusinessRole? BranchRole,
    Guid? InstructorId,
    bool IsBrandOwner,
    IReadOnlySet<string> Permissions,
    Guid? CustomRoleId = null)
{
    public bool HasPermission(string permission) => Permissions.Contains(permission);
}
