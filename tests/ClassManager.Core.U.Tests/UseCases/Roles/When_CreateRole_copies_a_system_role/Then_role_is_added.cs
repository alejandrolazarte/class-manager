using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Roles;

namespace ClassManager.Core.U.Tests.UseCases.Roles.When_CreateRole_copies_a_system_role;

public sealed class Then_role_is_added
{
    [Fact]
    public async Task Then_role_is_added_Run()
    {
        var builder = new RoleUseCaseBuilder();
        string[] permissions = [.. SystemRolePermissions.Of(BusinessRole.Instructor), Permissions.Payments.Record];

        var response = await builder.BuildCreate().ExecuteAsync(
            new CreateRoleCommand("Instructor que cobra", permissions, nameof(BusinessRole.Instructor)), CancellationToken.None);

        response.Value!.Permissions.ShouldBe(permissions, ignoreOrder: true);
        builder.AddedRoles.ShouldHaveSingleItem().CopiedFrom.ShouldBe(nameof(BusinessRole.Instructor));
    }
}
