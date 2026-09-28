using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.U.Tests.Domain.Authorization.When_permission_catalog_is_listed;

public sealed class Then_codes_are_unique
{
    [Fact]
    public void Then_codes_are_unique_Run()
    {
        Permissions.All.ShouldBeUnique();
    }
}
