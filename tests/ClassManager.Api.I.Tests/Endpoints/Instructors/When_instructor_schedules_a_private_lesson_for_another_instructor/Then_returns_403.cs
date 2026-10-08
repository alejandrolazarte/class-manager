namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_schedules_a_private_lesson_for_another_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.PostPrivateLessonAsync(
            scenario.OtherInstructorId, scenario.InstructorStudentId, InstructorScenario.ClassDate.AddDays(1));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
