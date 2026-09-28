namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_schedules_a_private_lesson_for_another_coach;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Coach.PostPrivateLessonAsync(
            scenario.OtherInstructorId, scenario.CoachStudentId, CoachScenario.ClassDate.AddDays(1));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
