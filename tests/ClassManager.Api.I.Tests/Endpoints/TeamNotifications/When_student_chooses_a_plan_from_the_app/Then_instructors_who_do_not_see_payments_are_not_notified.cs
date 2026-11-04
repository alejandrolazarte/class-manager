namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_student_chooses_a_plan_from_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_instructors_who_do_not_see_payments_are_not_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_instructors_who_do_not_see_payments_are_not_notified_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var plan = await scenario.Instructors.Business.HttpClient.CreateMonthlyPlanAsync();

        await scenario.Student.ChooseStudentAppPlanAsync(scenario.Instructors.InstructorStudentId, plan.Id);

        (await scenario.Instructors.Instructor.GetTeamNotificationsAsync())!.Items.ShouldBeEmpty();
    }
}
