namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPlans.When_student_without_a_plan_chooses_one;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_owes_it_this_month(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_family_owes_it_this_month_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var plan = await scenario.Instructors.Business.HttpClient.CreateMonthlyPlanAsync(price: FeeRequests.DefaultFee);

        await scenario.Student.ChooseStudentAppPlanAsync(scenario.Instructors.InstructorStudentId, plan.Id);

        var fees = (await scenario.Instructors.Business.HttpClient.GetMonthlyFeesAsync())!;
        fees.Clients.Single(client => client.ClientId == scenario.ClientId).Balance.ShouldBe(FeeRequests.DefaultFee);
    }
}
