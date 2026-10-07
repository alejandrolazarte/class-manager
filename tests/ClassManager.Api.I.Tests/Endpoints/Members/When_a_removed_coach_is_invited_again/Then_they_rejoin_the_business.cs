using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_a_removed_coach_is_invited_again;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_rejoin_the_business(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_rejoin_the_business_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Coach, instructor.Id);
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
        var coachMember = (await business.HttpClient.GetTeamAsync()).Members.Single(member => member.Role == BusinessRole.Coach);
        (await business.HttpClient.DeleteMemberAsync(coachMember.Id)).EnsureSuccessStatusCode();
        await business.HttpClient.InviteAsync(email, BusinessRole.Coach, instructor.Id);
        await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));

        var tokens = await anonymousClient.SignInAsync(email, MemberRequests.InviteePassword);

        using var coachClient = fixture.CreateClientWithToken(tokens.AccessToken);
        (await coachClient.GetCurrentMemberAsync()).BusinessId.ShouldBe(business.Business.Id);
    }
}
