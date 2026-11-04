namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_student_chooses_a_plan_from_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_owner_is_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_owner_is_notified_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var plan = await owner.CreateMonthlyPlanAsync();

        await scenario.Student.ChooseStudentAppPlanAsync(scenario.Instructors.InstructorStudentId, plan.Id);

        var notification = (await owner.GetTeamNotificationsAsync())!.Items.Single();
        notification.ShouldSatisfyAllConditions(
            () => notification.Title.ShouldBe($"Tomás eligió el plan {ClassPackRequests.MonthlyPlanName}"),
            () => notification.Url.ShouldBe($"/students/clients/{scenario.ClientId}"));
    }
}
