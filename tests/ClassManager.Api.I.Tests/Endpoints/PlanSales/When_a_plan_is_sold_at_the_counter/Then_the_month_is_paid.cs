using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.PlanSales.When_a_plan_is_sold_at_the_counter;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_month_is_paid(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_month_is_paid_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var studentId = await business.HttpClient.RegisterEnrolledStudentAsync();
        var plan = await business.HttpClient.CreateMonthlyPlanAsync(price: FeeRequests.DefaultFee);

        (await business.HttpClient.PostPlanSaleAsync(studentId, plan.Id)).EnsureSuccessStatusCode();

        var client = (await business.HttpClient.GetMonthlyFeesAsync())!.Clients.Single();
        client.ShouldSatisfyAllConditions(
            () => client.Status.ShouldBe(FeeStatus.Paid),
            () => client.Balance.ShouldBe(0m));
    }
}
