using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Roles;

namespace ClassManager.Core.U.Tests.Domain.Roles.When_CustomRole_is_created;

public sealed class Then_permissions_are_distinct_and_ordered
{
    [Fact]
    public void Then_permissions_are_distinct_and_ordered_Run()
    {
        var role = CustomRole.Create(
            "Coach que cobra",
            [Permissions.Payments.Record, Permissions.Sessions.ViewOwn, Permissions.Payments.Record],
            copiedFrom: "Coach",
            TestData.Now);

        role.Value!.Permissions.ShouldBe([Permissions.Payments.Record, Permissions.Sessions.ViewOwn]);
    }
}
