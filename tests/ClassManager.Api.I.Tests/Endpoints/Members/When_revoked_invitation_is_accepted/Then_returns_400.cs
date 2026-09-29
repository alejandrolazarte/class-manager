using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_revoked_invitation_is_accepted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        var invitation = await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        using (var revokeResponse = await business.HttpClient.DeleteAsync(new Uri($"{MemberRequests.InvitationsRoute}/{invitation.Id}", UriKind.Relative)))
        {
            revokeResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        using var anonymousClient = fixture.ApiFactory.CreateClient();
        using var response = await anonymousClient.PostAcceptInvitationAsync(fixture.ApiFactory.EmailSender.InvitationTokenSentTo(email));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
