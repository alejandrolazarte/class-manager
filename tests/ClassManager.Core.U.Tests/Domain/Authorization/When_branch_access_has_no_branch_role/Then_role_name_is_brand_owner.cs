using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Core.U.Tests.Domain.Authorization.When_branch_access_has_no_branch_role;

public sealed class Then_role_name_is_brand_owner
{
    [Fact]
    public void Then_role_name_is_brand_owner_Run()
    {
        var branch = new BranchAccess(Guid.CreateVersion7(), TestData.BusinessName, Guid.CreateVersion7(), null, IsBrandOwner: true);

        branch.RoleName.ShouldBe(nameof(OrganizationRole.BrandOwner));
    }
}
