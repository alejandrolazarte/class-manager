using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_invitation_is_resent;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_the_new_link_works(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_the_new_link_works_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        var invitation = await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        var firstToken = fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email);

        using (var resendResponse = await business.HttpClient.PostAsync(
            new Uri($"{MemberRequests.InvitationsRoute}/{invitation.Id}{ApiRoutes.Resend}", UriKind.Relative), null))
        {
            resendResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        using var anonymousClient = fixture.ApiFactory.CreateClient();
        using (var oldLinkResponse = await anonymousClient.PostAcceptInvitationAsync(firstToken))
        {
            oldLinkResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
    }
}
