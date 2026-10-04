namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_calls_a_team_endpoint;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        using var response = await scenario.Student.GetAsync(new Uri($"{ApiRoutes.Clients}/{scenario.ClientId}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
