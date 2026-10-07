namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_the_guardian_refuses_the_child;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_is_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_is_notified_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        var childEmail = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, childEmail, StudentAppRequests.ElevenYearsOld)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostRefuseGuardianConsentAsync(fixture.ApiFactory.EmailTransport.GuardianConsentTokenSentTo(clientEmail));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var notification = (await business.HttpClient.GetTeamNotificationsAsync())!.Items.Single();
        notification.Title.ShouldBe("No autorizaron a Tomás a usar la app");
        fixture.ApiFactory.EmailTransport.SentTo(childEmail).ShouldBeEmpty();
    }
}
