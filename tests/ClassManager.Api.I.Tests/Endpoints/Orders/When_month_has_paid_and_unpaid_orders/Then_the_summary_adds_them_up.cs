using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_month_has_paid_and_unpaid_orders;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_summary_adds_them_up(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_summary_adds_them_up_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 5);
        var classPack = await owner.CreateClassPackAsync();
        await owner.SellAtCounterAsync(scenario.FamilyId, [OrderRequests.PackLine(classPack.Id), OrderRequests.ProductLine(product.Variants[0].Id, 1)]);
        await scenario.Family.PlaceFamilyOrderAsync(FamilyShopRequests.ProductLine(product.Variants[0].Id, 2));

        var summary = await owner.GetOrderSummaryAsync();

        summary.ShouldBe(new OrderSummaryResponse(FeeRequests.CurrentMonth, 92m, 24m, 80m));
    }
}
