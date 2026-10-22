using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Abstractions.Security;

public sealed record MemberAccess(
    Guid AccountId,
    long? UserId,
    long BusinessId,
    BusinessRole? BranchRole,
    long? InstructorId,
    bool IsBrandOwner,
    IReadOnlySet<string> Permissions,
    long? CustomRoleId = null)
{
    public bool HasPermission(string permission) => Permissions.Contains(permission);
}
