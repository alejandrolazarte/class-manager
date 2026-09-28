using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Authorization.When_branch_owner_permissions_are_resolved;

public sealed class Then_every_permission_is_granted
{
    [Fact]
    public void Then_every_permission_is_granted_Run()
    {
        var permissions = SystemRolePermissions.Of(BusinessRole.BranchOwner);

        permissions.ShouldBe(Permissions.All, ignoreOrder: true);
    }
}
