namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_a_child_whose_user_was_renamed_declines_the_app_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_notice_names_the_user(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_notice_names_the_user_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var child = family.Students.Single();
        var childEmail = StudentAppRequests.UniqueStudentEmail();
        using (var invitation = await business.HttpClient.PostChildAppInvitationAsync(family.Id, child.Id, childEmail, StudentAppRequests.ChildBirthDate))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        await fixture.ChangeUserOfStudentAsync(business.Business.Id, child.Id, "Joaquín Pérez");
        using var anonymous = fixture.ApiFactory.CreateClient();
        (await anonymous.PostDeclineStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(childEmail))).EnsureSuccessStatusCode();

        var notification = (await business.HttpClient.GetTeamNotificationsAsync())!.Items.Single();
        notification.Title.ShouldBe("Joaquín rechazó la invitación a la app");
    }
}
