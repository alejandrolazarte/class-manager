namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassHistory.When_the_month_is_not_valid;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        using var response = await scenario.Student.GetClassHistoryResponseAsync(scenario.Instructors.InstructorStudentId, "2026-13");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
