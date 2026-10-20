using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Authorization.When_platform_reader_permissions_are_resolved;

public sealed class Then_only_platform_view_is_granted
{
    [Fact]
    public void Then_only_platform_view_is_granted_Run()
    {
        var permissions = SystemRolePermissions.Of(BusinessRole.PlatformReader);

        permissions.ShouldBe([Permissions.Platform.View]);
    }
}
