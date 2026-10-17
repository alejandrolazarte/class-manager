using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_a_team_member_deletes_their_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_no_longer_lists_them(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_no_longer_lists_them_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        using var anonymous = fixture.ApiFactory.CreateClient();
        var tokens = await anonymous.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
        using var member = fixture.CreateClientWithToken(tokens.AccessToken);

        using var response = await member.PostAccountDeletionAsync(MemberRequests.InviteePassword);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var team = await business.HttpClient.GetTeamAsync();
        team.Members.ShouldNotContain(teamMember => teamMember.Email == email);
    }
}
