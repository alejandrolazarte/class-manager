namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_instructor_leaves_a_comment_after_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_sees_it_on_home(ApiFixture fixture)
{
    private const string Comment = "Hoy mantuvo la postura en todo el circuito.";

    [Fact]
    public async Task Then_the_student_sees_it_on_home_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        using (var response = await scenario.Instructors.Instructor.PutFeedbackAsync(
            scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.Instructors.InstructorStudentId, Comment))
        {
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        var home = await scenario.Student.GetStudentAppHomeAsync();

        var feedback = home!.Students.Single().LatestFeedback;
        feedback.ShouldNotBeNull();
        feedback.Text.ShouldBe(Comment);
        feedback.InstructorFullName.ShouldBe(InstructorScenario.InstructorFullName);
        feedback.ClassName.ShouldBe(InstructorScenario.InstructorClassGroupName);
        feedback.Date.ShouldBe(InstructorScenario.ClassDate);
    }
}
