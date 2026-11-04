namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPlans.When_student_with_a_plan_chooses_another;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_changes_from_next_month(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_changes_from_next_month_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var studentId = scenario.Instructors.InstructorStudentId;
        var currentPlanId = await owner.GiveMonthlyPlanAsync(studentId);
        var otherPlan = await owner.CreateMonthlyPlanAsync("2 veces por semana", classCount: 8);

        await scenario.Student.ChooseStudentAppPlanAsync(studentId, otherPlan.Id);

        var student = await scenario.Student.PlanStudentAsync(studentId);
        student.ShouldSatisfyAllConditions(
            () => student.CurrentPlanId.ShouldBe(currentPlanId),
            () => student.NextMonthPlanId.ShouldBe(otherPlan.Id));
    }
}
