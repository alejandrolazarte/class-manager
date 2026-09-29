using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyShop.When_family_orders_the_last_unit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_reserved(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_reserved_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 1);

        var order = await scenario.Family.PlaceFamilyOrderAsync(FamilyShopRequests.ProductLine(product.Variants[0].Id, 1));

        order.Status.ShouldBe(OrderStatus.Requested);
        (await scenario.Family.GetFamilyShopAsync()).Products.Single().Variants[0].Availability.ShouldBe(StockAvailability.SoldOut);
        (await owner.ListProductsAsync()).Single().Variants[0].Stock.ShouldBe(0);
    }
}
