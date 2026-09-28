using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Abstractions.Security;

public sealed record MemberAccess(
    Guid UserId,
    Guid BusinessId,
    BusinessRole? BranchRole,
    bool IsBrandOwner,
    IReadOnlySet<string> Permissions)
{
    public bool HasPermission(string permission) => Permissions.Contains(permission);
}
