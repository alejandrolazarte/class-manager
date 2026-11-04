namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPlans.When_student_opens_the_shop_with_plans;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_plans_shown_in_the_shop_are_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_plans_shown_in_the_shop_are_listed_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        await owner.CreateMonthlyPlanAsync();
        await owner.CreateHiddenMonthlyPlanAsync();

        var shop = await scenario.Student.GetStudentAppShopAsync();

        shop.Plans.Select(plan => plan.Name).ShouldBe([ClassPackRequests.MonthlyPlanName]);
    }
}
