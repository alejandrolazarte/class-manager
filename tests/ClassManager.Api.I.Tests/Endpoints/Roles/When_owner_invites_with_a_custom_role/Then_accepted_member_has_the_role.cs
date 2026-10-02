using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_owner_invites_with_a_custom_role;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_accepted_member_has_the_role(ApiFixture fixture)
{
    [Fact]
    public async Task Then_accepted_member_has_the_role_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var role = await business.HttpClient.CreateRoleAsync(RoleRequests.CustomRoleName, [Permissions.Business.View, Permissions.Payments.ViewAll]);
        var email = MemberRequests.UniqueInviteeEmail();
        using (var invitation = await business.HttpClient.PostCustomRoleInvitationAsync(email, role.Id!.Value))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        await anonymous.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));

        var team = await business.HttpClient.GetTeamAsync();
        team.Members.ShouldContain(member => member.Email == email && member.CustomRoleId == role.Id);
    }
}
