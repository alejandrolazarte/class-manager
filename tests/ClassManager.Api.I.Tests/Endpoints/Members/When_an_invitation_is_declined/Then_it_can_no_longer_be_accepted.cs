using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_an_invitation_is_declined;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_can_no_longer_be_accepted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_can_no_longer_be_accepted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        var token = fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email);
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        using (var declined = await anonymousClient.PostDeclineInvitationAsync(token))
        {
            declined.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        using var response = await anonymousClient.PostAcceptInvitationAsync(token);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
