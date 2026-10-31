using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_month_has_paid_and_unpaid_orders;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_products_are_added_up(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_products_are_added_up_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 5);
        var classPack = await owner.CreateClassPackAsync();
        await owner.SellAtCounterAsync(scenario.ClientId, [OrderRequests.PackLine(classPack.Id), OrderRequests.ProductLine(product.Variants[0].Id, 1)]);
        await scenario.Student.PlaceStudentAppOrderAsync(StudentAppShopRequests.ProductLine(product.Variants[0].Id, 2));

        var summary = await owner.GetOrderSummaryAsync();

        summary.ShouldBe(new OrderSummaryResponse(FeeRequests.CurrentMonth, 12m, 24m));
    }
}
