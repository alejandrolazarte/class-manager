namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_gets_a_student_of_another_coach;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Coach.GetAsync(new Uri($"{ApiRoutes.Students}/{scenario.OtherStudentId}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
