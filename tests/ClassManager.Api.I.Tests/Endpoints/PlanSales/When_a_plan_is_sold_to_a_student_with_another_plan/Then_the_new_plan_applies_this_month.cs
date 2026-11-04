namespace ClassManager.Api.I.Tests.Endpoints.PlanSales.When_a_plan_is_sold_to_a_student_with_another_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_new_plan_applies_this_month(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_new_plan_applies_this_month_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var studentId = await business.HttpClient.RegisterEnrolledStudentAsync();
        await business.HttpClient.GiveMonthlyPlanAsync(studentId, price: 8000m);
        var plan = await business.HttpClient.CreateMonthlyPlanAsync("Plan nuevo", price: FeeRequests.DefaultFee);

        (await business.HttpClient.PostPlanSaleAsync(studentId, plan.Id, paidAmount: 0m)).EnsureSuccessStatusCode();

        (await business.HttpClient.GetMonthlyFeesAsync())!.Clients.Single().Balance.ShouldBe(FeeRequests.DefaultFee);
    }
}
