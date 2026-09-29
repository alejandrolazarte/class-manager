using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Roles;
using ClassManager.Core.UseCases.Roles;

namespace ClassManager.Core.U.Tests.UseCases.Roles.When_CreateRole_with_permissions_the_member_lacks;

public sealed class Then_returns_forbidden
{
    [Fact]
    public async Task Then_returns_forbidden_Run()
    {
        var builder = new RoleUseCaseBuilder();
        builder.ActAs(new HashSet<string> { Permissions.Roles.Manage, Permissions.Business.View }, BusinessRole.Custom, Guid.CreateVersion7());

        var response = await builder.BuildCreate().ExecuteAsync(
            new CreateRoleCommand("Cobranzas", [Permissions.Business.View, Permissions.Payments.Record], null), CancellationToken.None);

        response.Error!.Code.ShouldBe(RoleErrorCodes.ExceedsOwn);
        builder.AddedRoles.ShouldBeEmpty();
    }
}
