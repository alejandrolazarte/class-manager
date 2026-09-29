using ClassManager.Core.Domain.Roles;
using ClassManager.Core.UseCases.Roles;

namespace ClassManager.Core.U.Tests.UseCases.Roles.When_DeleteRole_in_use;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new RoleUseCaseBuilder();
        builder.Members
            .Setup(repository => repository.ListByCustomRoleAsync(builder.ExistingRole.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([RoleUseCaseBuilder.MemberWith(builder.ExistingRole, instructorId: null)]);

        var response = await builder.BuildDelete().ExecuteAsync(new DeleteRoleCommand(builder.ExistingRole.Id), CancellationToken.None);

        response.Error!.Code.ShouldBe(RoleErrorCodes.InUse);
        builder.CustomRoles.Verify(repository => repository.Remove(It.IsAny<CustomRole>()), Times.Never);
    }
}
