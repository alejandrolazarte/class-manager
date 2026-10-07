using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_pending_invitation_is_checked;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_the_invited_email(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_the_invited_email_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        using var response = await anonymousClient.PostCheckInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));

        var checkedInvitation = await response.Content.ReadFromJsonAsync<CheckInvitationResponse>(ApiRequests.JsonOptions);
        checkedInvitation.ShouldBe(new CheckInvitationResponse(email, checkedInvitation!.BusinessName, HasAccount: false));
    }
}
