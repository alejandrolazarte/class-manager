namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_instructor_comments_a_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_gets_a_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_gets_a_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();

        (await scenario.Instructors.Instructor.PutFeedbackAsync(
            scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.Instructors.InstructorStudentId, "Muy buena patada")).EnsureSuccessStatusCode();

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.ShouldBe(new PushMessage($"{InstructorScenario.InstructorFullName} comentó la clase de Tomás", "Muy buena patada", "/student-app/news"));
    }
}
