namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_the_client_authorizes_from_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_child_is_invited(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_child_is_invited_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var clientApp = await fixture.SignInClientOfFamilyAsync(business.HttpClient, family.Id, clientEmail);
        var childEmail = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, childEmail, StudentAppRequests.ElevenYearsOld)).EnsureSuccessStatusCode();
        var home = await clientApp.GetStudentAppHomeAsync();

        using var response = await clientApp.PostGuardianConsentInAppAsync(home!.PendingGuardianConsents.Single().InvitationId);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(childEmail).ShouldNotBeNullOrEmpty();
    }
}
