namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPlans.When_student_cancels_a_plan_that_starts_next_month;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_nothing_is_owed_next_month(ApiFixture fixture)
{
    [Fact]
    public async Task Then_nothing_is_owed_next_month_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var studentId = scenario.Instructors.InstructorStudentId;
        var plan = await owner.CreateMonthlyPlanAsync();
        (await owner.PutStudentPlanAsync(studentId, plan.Id, effectiveFrom: FeeRequests.NextMonth)).EnsureSuccessStatusCode();

        await scenario.Student.ChooseStudentAppPlanAsync(studentId, null);

        var fees = (await owner.GetMonthlyFeesAsync(FeeRequests.NextMonth))!;
        fees.Clients.ShouldNotContain(client => client.ClientId == scenario.ClientId);
    }
}
