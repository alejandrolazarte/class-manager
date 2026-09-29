using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Roles;
using ClassManager.Core.UseCases.Roles;

namespace ClassManager.Core.U.Tests.UseCases.Roles.When_UpdateRole_of_own_role;

public sealed class Then_returns_forbidden
{
    [Fact]
    public async Task Then_returns_forbidden_Run()
    {
        var builder = new RoleUseCaseBuilder();
        builder.ActAs(SystemRolePermissions.Of(BusinessRole.BranchOwner), BusinessRole.Custom, builder.ExistingRole.Id);

        var response = await builder.BuildUpdate().ExecuteAsync(
            new UpdateRoleCommand(builder.ExistingRole.Id, "Recepción", [Permissions.Business.View]), CancellationToken.None);

        response.Error!.Code.ShouldBe(RoleErrorCodes.OwnRole);
    }
}
