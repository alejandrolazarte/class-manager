using ClassManager.Core.Domain.Images;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_product_image_is_larger_than_the_limit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();

        using var response = await business.HttpClient.PutProductImageAsync(product.Id, CatalogImageRequests.OversizedPngImage());

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(ImageErrorCodes.TooLarge);
    }
}
