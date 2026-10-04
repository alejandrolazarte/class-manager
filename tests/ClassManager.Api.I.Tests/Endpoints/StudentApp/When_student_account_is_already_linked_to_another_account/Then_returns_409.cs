namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_account_is_already_linked_to_another_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var firstClient = await fixture.InviteStudentAppOfAsync(coaches, CoachScenario.CoachStudentFullName);
        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var otherClientId = fees!.Clients.Single(client => client.StudentNames.Contains(CoachScenario.OtherStudentFullName)).ClientId;
        using (var invitation = await coaches.Business.HttpClient.PostStudentAppInvitationAsync(otherClientId, firstClient.Email))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var response = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(firstClient.Email));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
