namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_instructor_turned_on_notifications;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_instructor_gets_a_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_instructor_gets_a_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.Instructors.Instructor.SubscribeToTeamPushAsync();

        (await scenario.Student.PutAbsenceAsync(
            scenario.Instructors.InstructorStudentId, scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate.AddDays(7))).EnsureSuccessStatusCode();

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.Title.ShouldBe("Tomás avisó que no viene");
    }
}
