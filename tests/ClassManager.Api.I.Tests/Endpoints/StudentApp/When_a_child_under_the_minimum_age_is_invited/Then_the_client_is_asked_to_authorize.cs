namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_child_under_the_minimum_age_is_invited;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_client_is_asked_to_authorize(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_client_is_asked_to_authorize_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        var childEmail = StudentAppRequests.UniqueStudentEmail();

        using var response = await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, childEmail, StudentAppRequests.ElevenYearsOld);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        fixture.ApiFactory.EmailTransport.GuardianConsentTokenSentTo(clientEmail).ShouldNotBeNullOrEmpty();
        fixture.ApiFactory.EmailTransport.SentTo(childEmail).ShouldBeEmpty();
    }
}
