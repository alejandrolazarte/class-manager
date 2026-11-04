namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_student_cancels_its_plan_from_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_owner_is_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_owner_is_notified_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        await owner.GiveMonthlyPlanAsync(scenario.Instructors.InstructorStudentId);

        await scenario.Student.ChooseStudentAppPlanAsync(scenario.Instructors.InstructorStudentId, null);

        var notification = (await owner.GetTeamNotificationsAsync())!.Items.Single();
        notification.Title.ShouldBe("Tomás dio de baja su plan");
    }
}
