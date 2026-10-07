using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_someone_declines_a_team_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_person_who_invited_is_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_person_who_invited_is_notified_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        using var anonymous = fixture.ApiFactory.CreateClient();

        (await anonymous.PostDeclineInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email))).EnsureSuccessStatusCode();

        var notification = (await business.HttpClient.GetTeamNotificationsAsync())!.Items.Single();
        notification.Title.ShouldBe($"{email} rechazó la invitación al equipo");
        notification.Url.ShouldBe("/settings/team");
    }
}
