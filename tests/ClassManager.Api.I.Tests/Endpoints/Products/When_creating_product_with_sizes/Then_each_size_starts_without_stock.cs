using ClassManager.Core.Domain.Products;

namespace ClassManager.Api.I.Tests.Endpoints.Products.When_creating_product_with_sizes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_size_starts_without_stock(ApiFixture fixture)
{
    [Fact]
    public async Task Then_each_size_starts_without_stock_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        await business.HttpClient.CreateProductAsync(StockMode.Tracked, "S", "M", "L");

        var product = (await business.HttpClient.ListProductsAsync()).Single();
        product.Variants.Select(variant => variant.Name).ShouldBe(["S", "M", "L"]);
        product.Variants.ShouldAllBe(variant => variant.Stock == 0);
    }
}
