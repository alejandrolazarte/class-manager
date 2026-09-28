using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Branches;

public sealed record BranchResponse(
    Guid BusinessId,
    string Name,
    BusinessRole? BranchRole,
    bool IsBrandOwner,
    bool IsCurrent)
{
    public static BranchResponse From(BranchAccess branch, Guid currentBusinessId) =>
        new(branch.BusinessId, branch.BusinessName, branch.BranchRole, branch.IsBrandOwner, branch.BusinessId == currentBusinessId);
}
