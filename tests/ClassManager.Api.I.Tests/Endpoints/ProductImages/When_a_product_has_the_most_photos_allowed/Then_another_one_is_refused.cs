using ClassManager.Core.Domain.Images;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_a_product_has_the_most_photos_allowed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_another_one_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_another_one_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();
        for (var photoNumber = 0; photoNumber < Product.MaximumImages; photoNumber++)
        {
            await business.HttpClient.AddProductImageAsync(product.Id);
        }

        using var response = await business.HttpClient.PostProductImageAsync(product.Id, CatalogImageRequests.PngImage);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(ImageErrorCodes.LimitReached);
    }
}
