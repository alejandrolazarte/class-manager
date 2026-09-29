using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Roles;

namespace ClassManager.Core.U.Tests.Domain.Roles.When_CustomRole_has_a_brand_permission;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var role = CustomRole.Create("Encargada", [Permissions.Brand.CreateBranches], copiedFrom: null, TestData.Now);

        role.Error!.Code.ShouldBe(RoleErrorCodes.PermissionNotAssignable);
    }
}
