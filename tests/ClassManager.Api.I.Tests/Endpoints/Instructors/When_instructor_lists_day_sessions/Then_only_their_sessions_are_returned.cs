namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_lists_day_sessions;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_sessions_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_sessions_are_returned_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        var sessions = await scenario.Instructor.GetDayAsync(InstructorScenario.ClassDate);

        sessions!.Select(session => session.ClassGroupName).ShouldBe([InstructorScenario.InstructorClassGroupName]);
    }
}
