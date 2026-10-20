using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Authorization.When_platform_operator_permissions_are_resolved;

public sealed class Then_only_platform_permissions_are_granted
{
    [Fact]
    public void Then_only_platform_permissions_are_granted_Run()
    {
        var permissions = SystemRolePermissions.Of(BusinessRole.PlatformOperator);

        permissions.ShouldBe([Permissions.Platform.View, Permissions.Platform.Manage], ignoreOrder: true);
    }
}
