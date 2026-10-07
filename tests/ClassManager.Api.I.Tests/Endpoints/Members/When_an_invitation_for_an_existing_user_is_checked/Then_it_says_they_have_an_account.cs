using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_an_invitation_for_an_existing_user_is_checked;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_says_they_have_an_account(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_says_they_have_an_account_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = AuthenticationRequests.UniqueEmail();
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        await anonymousClient.SignUpAsync(AuthenticationRequests.SignUpCommand(email));
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);

        using var response = await anonymousClient.PostCheckInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));

        var checkedInvitation = await response.Content.ReadFromJsonAsync<CheckInvitationResponse>(ApiRequests.JsonOptions);
        checkedInvitation!.HasAccount.ShouldBeTrue();
    }
}
