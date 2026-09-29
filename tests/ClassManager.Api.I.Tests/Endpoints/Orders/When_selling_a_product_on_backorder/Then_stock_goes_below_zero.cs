using ClassManager.Core.Domain.Products;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_selling_a_product_on_backorder;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_stock_goes_below_zero(ApiFixture fixture)
{
    [Fact]
    public async Task Then_stock_goes_below_zero_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync(StockMode.TrackedWithBackorder);

        await business.HttpClient.SellAtCounterAsync(null, [OrderRequests.ProductLine(product.Variants[0].Id, 2)], isDelivered: false);

        (await business.HttpClient.ListProductsAsync()).Single().Variants[0].Stock.ShouldBe(-2);
    }
}
