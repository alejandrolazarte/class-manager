using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_unused_role_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_no_longer_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_no_longer_listed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var role = await business.HttpClient.CreateRoleAsync(RoleRequests.CustomRoleName, [Permissions.Business.View]);
        using (var invitation = await business.HttpClient.PostCustomRoleInvitationAsync(MemberRequests.UniqueInviteeEmail(), role.Id!.Value))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var team = await business.HttpClient.GetTeamAsync();
        using (var revoke = await business.HttpClient.DeleteAsync(
            new Uri($"{ApiRoutes.Members}{ApiRoutes.InvitationsSegment}/{team.Invitations[0].Id}", UriKind.Relative)))
        {
            revoke.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        using var response = await business.HttpClient.DeleteRoleAsync(role.Id.Value);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.ListRolesAsync()).ShouldNotContain(listed => listed.Id == role.Id);
    }
}
