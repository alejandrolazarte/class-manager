using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Api.I.Tests.Endpoints.Products.When_creating_product_with_initial_stock;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_size_has_its_units(ApiFixture fixture)
{
    [Fact]
    public async Task Then_each_size_has_its_units_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostAsJsonAsync(
            ApiRoutes.Products,
            new CreateProductCommand(
                ProductRequests.ProductName,
                null,
                ProductRequests.ProductPrice,
                StockMode.Tracked,
                true,
                [new VariantChange(null, "S", InitialStock: 3), new VariantChange(null, "M", InitialStock: 0), new VariantChange(null, "L", InitialStock: 2)]),
            ApiRequests.JsonOptions);

        var created = (await response.Content.ReadFromJsonAsync<ProductResponse>(ApiRequests.JsonOptions))!;
        created.Variants.Select(variant => variant.Stock).ShouldBe([3, 0, 2]);
        (await business.HttpClient.ListStockMovementsAsync(created.Id)).Count.ShouldBe(2);
    }
}
