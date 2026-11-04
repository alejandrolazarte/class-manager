namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_student_chooses_the_plan_it_already_has;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_nobody_is_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_nobody_is_notified_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var planId = await owner.GiveMonthlyPlanAsync(scenario.Instructors.InstructorStudentId);

        await scenario.Student.ChooseStudentAppPlanAsync(scenario.Instructors.InstructorStudentId, planId);

        (await owner.GetTeamNotificationsAsync())!.Items.ShouldBeEmpty();
    }
}
