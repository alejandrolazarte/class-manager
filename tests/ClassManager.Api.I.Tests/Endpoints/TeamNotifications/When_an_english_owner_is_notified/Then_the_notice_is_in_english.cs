using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_an_english_owner_is_notified;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_notice_is_in_english(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_notice_is_in_english_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(
            fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand(AuthenticationRequests.UniqueEmail()));
        using var owner = fixture.CreateClientWithToken(signUp.AccessToken);
        await fixture.ChangePlanAsync((await owner.GetCurrentMemberAsync()).BusinessId, PlanCodes.Pro);
        (await owner.PutMyLanguageAsync(AccountLanguage.English)).EnsureSuccessStatusCode();
        var invitee = MemberRequests.UniqueInviteeEmail();
        await owner.InviteAsync(invitee, BusinessRole.Viewer);

        (await anonymous.PostDeclineInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(invitee))).EnsureSuccessStatusCode();

        var notification = (await owner.GetTeamNotificationsAsync())!.Items.Single();
        notification.Title.ShouldBe($"{invitee} declined the team invitation");
    }
}
