namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_the_guardian_authorizes_the_child;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_child_creates_the_account(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_child_creates_the_account_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        var childEmail = StudentAppRequests.UniqueStudentEmail();
        (await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, childEmail, StudentAppRequests.ElevenYearsOld)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();
        (await anonymous.PostGiveGuardianConsentAsync(fixture.ApiFactory.EmailTransport.GuardianConsentTokenSentTo(clientEmail))).EnsureSuccessStatusCode();

        using var response = await anonymous.PostAcceptStudentAppInvitationAsync(
            fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(childEmail), StudentAppRequests.ElevenYearsOld);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
