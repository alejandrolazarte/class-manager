using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_invitation_is_accepted_twice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_second_attempt_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_second_attempt_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        var token = fixture.ApiFactory.EmailSender.InvitationTokenSentTo(email);
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        await anonymousClient.AcceptInvitationAsync(token);

        using var response = await anonymousClient.PostAcceptInvitationAsync(token);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
