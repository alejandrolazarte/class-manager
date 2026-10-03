using ClassManager.Core.Domain.Images;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_product_image_is_not_an_image;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();

        using var response = await business.HttpClient.PutProductImageAsync(product.Id, "not an image"u8.ToArray());

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(ImageErrorCodes.Unsupported);
    }
}
