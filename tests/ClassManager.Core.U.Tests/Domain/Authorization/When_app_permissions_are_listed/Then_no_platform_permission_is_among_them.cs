using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.U.Tests.Domain.Authorization.When_app_permissions_are_listed;

public sealed class Then_no_platform_permission_is_among_them
{
    [Fact]
    public void Then_no_platform_permission_is_among_them_Run()
    {
        Permissions.All.Intersect(Permissions.PlatformOnly).ShouldBeEmpty();
    }
}
