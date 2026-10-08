using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Roles;
using ClassManager.Core.UseCases.Roles;

namespace ClassManager.Core.U.Tests.UseCases.Roles.When_UpdateRole_needs_a_linked_instructor_that_members_lack;

public sealed class Then_returns_conflict
{
    [Fact]
    public async Task Then_returns_conflict_Run()
    {
        var builder = new RoleUseCaseBuilder();
        builder.Members
            .Setup(repository => repository.ListByCustomRoleAsync(builder.ExistingRole.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([RoleUseCaseBuilder.MemberWith(builder.ExistingRole, instructorId: null)]);

        var response = await builder.BuildUpdate().ExecuteAsync(
            new UpdateRoleCommand(builder.ExistingRole.Id, "Recepción", [Permissions.Business.View, Permissions.Sessions.ViewOwn]),
            CancellationToken.None);

        response.Error!.Code.ShouldBe(RoleErrorCodes.InstructorRequired);
        response.Error.Details[RoleErrorCodes.MemberCountDetail].ShouldBe(1);
    }
}
