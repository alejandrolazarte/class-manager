using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Api.I.Tests.Endpoints.Products.When_creating_product_with_sizes_and_colors;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_combination_has_its_units(ApiFixture fixture)
{
    [Fact]
    public async Task Then_each_combination_has_its_units_Run()
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
                [
                    new VariantChange(null, "S", ProductColors.Black, 4),
                    new VariantChange(null, "S", ProductColors.White, 0),
                    new VariantChange(null, "M", ProductColors.Black, 6),
                    new VariantChange(null, "M", ProductColors.White, 8),
                ]),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var product = (await business.HttpClient.ListProductsAsync()).Single();
        product.Variants.Select(variant => (variant.Size, variant.Color, variant.Stock)).ShouldBe(
        [
            ("S", ProductColors.Black, 4),
            ("S", ProductColors.White, 0),
            ("M", ProductColors.Black, 6),
            ("M", ProductColors.White, 8),
        ]);
    }
}
