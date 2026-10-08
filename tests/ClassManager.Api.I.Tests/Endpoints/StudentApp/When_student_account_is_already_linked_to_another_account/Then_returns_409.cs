namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_account_is_already_linked_to_another_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var instructors = await fixture.SeedInstructorScenarioAsync();
        var firstClient = await fixture.InviteStudentAppOfAsync(instructors, InstructorScenario.InstructorStudentFullName);
        var fees = await instructors.Business.HttpClient.GetMonthlyFeesAsync();
        var otherClientId = fees!.Clients.Single(client => client.StudentNames.Contains(InstructorScenario.OtherStudentFullName)).ClientId;
        using (var invitation = await instructors.Business.HttpClient.PostStudentAppInvitationAsync(otherClientId, firstClient.Email))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var response = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(firstClient.Email));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
