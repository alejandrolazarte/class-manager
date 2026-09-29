using ClassManager.Core.Domain.Products;

namespace ClassManager.Api.I.Tests.Endpoints.Products.When_loading_and_adjusting_stock;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_stock_is_the_sum_of_movements(ApiFixture fixture)
{
    [Fact]
    public async Task Then_stock_is_the_sum_of_movements_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();
        await business.HttpClient.RestockAsync(product, 5);

        using var response = await business.HttpClient.PostStockMovementAsync(
            product.Id, product.Variants[0].Id, StockMovementKind.Adjustment, -1, "Llegó roto");

        (await business.HttpClient.ListProductsAsync()).Single().Variants[0].Stock.ShouldBe(4);
        (await business.HttpClient.ListStockMovementsAsync(product.Id)).Count.ShouldBe(2);
    }
}
