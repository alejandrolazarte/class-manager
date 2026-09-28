using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Authorization.When_viewer_permissions_are_resolved;

public sealed class Then_only_view_permissions_are_granted
{
    [Fact]
    public void Then_only_view_permissions_are_granted_Run()
    {
        var permissions = SystemRolePermissions.Of(BusinessRole.Viewer);

        permissions.ShouldAllBe(permission => permission.Contains(".view", StringComparison.Ordinal));
    }
}
