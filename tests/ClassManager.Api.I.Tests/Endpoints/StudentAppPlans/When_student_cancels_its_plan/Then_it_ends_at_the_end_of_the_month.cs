namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPlans.When_student_cancels_its_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_ends_at_the_end_of_the_month(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_ends_at_the_end_of_the_month_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var studentId = scenario.Instructors.InstructorStudentId;
        var planId = await scenario.Instructors.Business.HttpClient.GiveMonthlyPlanAsync(studentId);

        await scenario.Student.ChooseStudentAppPlanAsync(studentId, null);

        var student = await scenario.Student.PlanStudentAsync(studentId);
        student.ShouldSatisfyAllConditions(
            () => student.CurrentPlanId.ShouldBe(planId),
            () => student.NextMonthPlanId.ShouldBeNull());
    }
}
