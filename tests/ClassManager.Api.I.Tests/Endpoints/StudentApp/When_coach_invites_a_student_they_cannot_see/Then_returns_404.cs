namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_coach_invites_a_student_they_cannot_see;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedCollectorScenarioAsync();

        using var response = await scenario.Coaches.Coach.PostStudentAppInvitationAsync(scenario.CollectorClientId, StudentAppRequests.UniqueStudentEmail());

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
