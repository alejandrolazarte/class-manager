using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_a_removed_coach_refreshes_the_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_401(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_401_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Coach, instructor.Id);
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
        var tokens = await anonymousClient.SignInAsync(email, MemberRequests.InviteePassword);
        var coachMember = (await business.HttpClient.GetTeamAsync()).Members.Single(member => member.Role == BusinessRole.Coach);
        (await business.HttpClient.DeleteMemberAsync(coachMember.Id)).EnsureSuccessStatusCode();

        using var refreshResponse = await anonymousClient.PostRefreshAsync(tokens.RefreshToken);

        refreshResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
