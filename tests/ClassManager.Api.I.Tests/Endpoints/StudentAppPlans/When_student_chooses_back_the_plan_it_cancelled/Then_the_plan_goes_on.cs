namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPlans.When_student_chooses_back_the_plan_it_cancelled;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_plan_goes_on(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_plan_goes_on_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var studentId = scenario.Instructors.InstructorStudentId;
        var planId = await scenario.Instructors.Business.HttpClient.GiveMonthlyPlanAsync(studentId);
        await scenario.Student.ChooseStudentAppPlanAsync(studentId, null);

        await scenario.Student.ChooseStudentAppPlanAsync(studentId, planId);

        (await scenario.Student.PlanStudentAsync(studentId)).NextMonthPlanId.ShouldBe(planId);
    }
}
