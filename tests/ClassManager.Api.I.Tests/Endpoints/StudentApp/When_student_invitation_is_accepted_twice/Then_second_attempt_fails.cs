namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_invitation_is_accepted_twice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_second_attempt_fails(ApiFixture fixture)
{
    [Fact]
    public async Task Then_second_attempt_fails_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(scenario.Email));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
