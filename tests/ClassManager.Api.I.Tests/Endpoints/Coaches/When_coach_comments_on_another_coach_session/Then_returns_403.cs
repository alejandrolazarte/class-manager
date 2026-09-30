namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_comments_on_another_coach_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Coach.PutFeedbackAsync(
            scenario.OtherClassGroup.Id, CoachScenario.ClassDate, scenario.OtherStudentId, "No es mi clase");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
