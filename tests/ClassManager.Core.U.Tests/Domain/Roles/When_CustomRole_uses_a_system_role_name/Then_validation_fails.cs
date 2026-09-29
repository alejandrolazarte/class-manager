using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Roles;

namespace ClassManager.Core.U.Tests.Domain.Roles.When_CustomRole_uses_a_system_role_name;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var role = CustomRole.Create(" solo lectura ", [Permissions.Business.View], copiedFrom: null, TestData.Now);

        role.Error!.Code.ShouldBe(RoleErrorCodes.NameReserved);
    }
}
