namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_schedules_a_private_lesson_for_a_student_they_cannot_see;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.PostPrivateLessonAsync(
            scenario.InstructorId, scenario.OtherStudentId, InstructorScenario.ClassDate.AddDays(1));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
