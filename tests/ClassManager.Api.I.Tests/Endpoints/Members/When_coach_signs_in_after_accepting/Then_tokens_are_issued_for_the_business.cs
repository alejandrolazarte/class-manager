using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_coach_signs_in_after_accepting;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_tokens_are_issued_for_the_business(ApiFixture fixture)
{
    [Fact]
    public async Task Then_tokens_are_issued_for_the_business_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Coach, instructor.Id);
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));

        var tokens = await anonymousClient.SignInAsync(email, MemberRequests.InviteePassword);

        using var coachClient = fixture.CreateClientWithToken(tokens.AccessToken);
        (await coachClient.GetCurrentMemberAsync()).BusinessId.ShouldBe(business.Business.Id);
    }
}
