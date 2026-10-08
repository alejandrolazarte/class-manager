using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_an_existing_user_accepts;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_join_the_team(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_join_the_team_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = AuthenticationRequests.UniqueEmail();
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        await anonymousClient.SignUpAsync(AuthenticationRequests.SignUpCommand(email));
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);

        using var response = await anonymousClient.PostAcceptInvitationAsync(
            fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email), password: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var team = await business.HttpClient.GetTeamAsync();
        team.Members.ShouldContain(member => member.Email == email);
    }
}
