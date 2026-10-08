using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.U.Tests.Domain.Authorization.When_instructor_permissions_are_resolved;

public sealed class Then_money_and_settings_are_not_granted
{
    [Fact]
    public void Then_money_and_settings_are_not_granted_Run()
    {
        var permissions = SystemRolePermissions.Of(BusinessRole.Instructor);

        permissions.ShouldNotContain(Permissions.Payments.ViewAll);
        permissions.ShouldNotContain(Permissions.Payments.Record);
        permissions.ShouldNotContain(Permissions.Business.Manage);
        permissions.ShouldNotContain(Permissions.Members.View);
        permissions.ShouldNotContain(Permissions.Sessions.ViewAll);
        permissions.ShouldContain(Permissions.Sessions.ViewOwn);
        permissions.ShouldContain(Permissions.Students.Manage);
    }
}
