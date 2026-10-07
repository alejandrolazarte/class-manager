namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_a_child_declines_the_app_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_person_who_invited_is_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_person_who_invited_is_notified_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var childEmail = StudentAppRequests.UniqueStudentEmail();
        using (var invitation = await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, childEmail, StudentAppRequests.ChildBirthDate))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        (await anonymous.PostDeclineStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(childEmail))).EnsureSuccessStatusCode();

        var notification = (await business.HttpClient.GetTeamNotificationsAsync())!.Items.Single();
        notification.Title.ShouldBe("Tomás rechazó la invitación a la app");
        notification.Url.ShouldBe($"/students/clients/{family.Id}");
    }
}
