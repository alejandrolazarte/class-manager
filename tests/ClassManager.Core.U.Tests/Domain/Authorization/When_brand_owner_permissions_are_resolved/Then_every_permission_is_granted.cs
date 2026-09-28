using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.U.Tests.Domain.Authorization.When_brand_owner_permissions_are_resolved;

public sealed class Then_every_permission_is_granted
{
    [Fact]
    public void Then_every_permission_is_granted_Run()
    {
        SystemRolePermissions.BrandOwner.ShouldBe(Permissions.All, ignoreOrder: true);
    }
}
