using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_coach_opens_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_their_name_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_their_name_is_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Coach, instructor.Id);
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
        var tokens = await anonymousClient.SignInAsync(email, MemberRequests.InviteePassword);
        using var coachClient = fixture.CreateClientWithToken(tokens.AccessToken);

        var member = await coachClient.GetCurrentMemberAsync();

        member.FullName.ShouldBe(MemberRequests.InviteeFullName);
    }
}
