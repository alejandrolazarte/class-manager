namespace ClassManager.Api.I.Tests.Endpoints.PlanSales.When_a_plan_is_sold_paying_a_part;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_rest_is_owed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_rest_is_owed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var studentId = await business.HttpClient.RegisterEnrolledStudentAsync();
        var plan = await business.HttpClient.CreateMonthlyPlanAsync(price: FeeRequests.DefaultFee);

        (await business.HttpClient.PostPlanSaleAsync(studentId, plan.Id, paidAmount: 5000m)).EnsureSuccessStatusCode();

        (await business.HttpClient.GetMonthlyFeesAsync())!.Clients.Single().Balance.ShouldBe(FeeRequests.DefaultFee - 5000m);
    }
}
