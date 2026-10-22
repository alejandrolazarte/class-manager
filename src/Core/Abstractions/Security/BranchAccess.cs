using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Core.Abstractions.Security;

public sealed record BranchAccess(
    long BusinessId,
    string BusinessName,
    long OrganizationId,
    BusinessRole? BranchRole,
    bool IsBrandOwner,
    string? CustomRoleName = null)
{
    public string RoleName => BranchRole?.ToString() ?? nameof(OrganizationRole.BrandOwner);
}
