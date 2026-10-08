namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_gets_a_private_lesson_of_another_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var lesson = await scenario.Business.HttpClient.SchedulePrivateLessonAsync(
            scenario.OtherInstructorId, scenario.OtherStudentId, InstructorScenario.ClassDate.AddDays(1));

        using var response = await scenario.Instructor.GetAsync(new Uri($"{ApiRoutes.PrivateLessons}/{lesson.Id}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
