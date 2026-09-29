namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_assigns_a_substitute;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Coach.PutSubstituteAsync(
            scenario.CoachClassGroup.Id, CoachScenario.ClassDate, scenario.OtherInstructorId);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
