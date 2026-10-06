namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_the_client_is_invited_with_a_new_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_first_invitation_no_longer_works(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_first_invitation_no_longer_works_Run()
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.Clients.Single(client => client.StudentNames.Contains(CoachScenario.CoachStudentFullName)).ClientId;
        var firstEmail = StudentAppRequests.UniqueStudentEmail();
        var secondEmail = StudentAppRequests.UniqueStudentEmail();
        using var firstInvitation = await coaches.Business.HttpClient.PostStudentAppInvitationAsync(clientId, firstEmail);
        using var secondInvitation = await coaches.Business.HttpClient.PostStudentAppInvitationAsync(clientId, secondEmail);

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var response = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(firstEmail));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
