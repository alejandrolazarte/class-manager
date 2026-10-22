using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Branches;

public sealed record BranchResponse(
    long BusinessId,
    string Name,
    BusinessRole? BranchRole,
    bool IsBrandOwner,
    bool IsCurrent,
    string? CustomRoleName = null)
{
    public static BranchResponse From(BranchAccess branch, long currentBusinessId) =>
        new(branch.BusinessId, branch.BusinessName, branch.BranchRole, branch.IsBrandOwner, branch.BusinessId == currentBusinessId, branch.CustomRoleName);
}
