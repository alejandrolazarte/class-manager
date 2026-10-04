namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_used_student_invitation_is_checked;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostCheckStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(scenario.Email));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
