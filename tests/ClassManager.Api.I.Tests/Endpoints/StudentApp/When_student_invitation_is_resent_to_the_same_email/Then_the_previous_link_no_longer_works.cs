namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_invitation_is_resent_to_the_same_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_previous_link_no_longer_works(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_previous_link_no_longer_works_Run()
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.Clients.Single(client => client.StudentNames.Contains(CoachScenario.CoachStudentFullName)).ClientId;
        var email = StudentAppRequests.UniqueStudentEmail();
        using var previousInvitation = await coaches.Business.HttpClient.PostStudentAppInvitationAsync(clientId, email);
        var previousToken = fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(email);
        using var resentInvitation = await coaches.Business.HttpClient.PostStudentAppInvitationAsync(clientId, email.ToUpperInvariant());

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var response = await anonymous.PostAcceptStudentAppInvitationAsync(previousToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
