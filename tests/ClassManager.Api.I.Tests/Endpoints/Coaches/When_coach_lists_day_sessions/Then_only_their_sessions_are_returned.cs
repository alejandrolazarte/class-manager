namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_lists_day_sessions;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_sessions_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_sessions_are_returned_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        var sessions = await scenario.Coach.GetDayAsync(CoachScenario.ClassDate);

        sessions!.Select(session => session.ClassGroupName).ShouldBe([CoachScenario.CoachClassGroupName]);
    }
}
