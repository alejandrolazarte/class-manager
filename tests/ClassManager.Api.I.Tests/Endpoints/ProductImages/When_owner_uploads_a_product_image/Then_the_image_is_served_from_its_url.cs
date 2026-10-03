namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_owner_uploads_a_product_image;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_image_is_served_from_its_url(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_image_is_served_from_its_url_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();

        var updatedProduct = await business.HttpClient.SetProductImageAsync(product.Id);

        using var image = await fixture.AnonymousClient.GetAsync(new Uri(updatedProduct.ImageUrl!));
        (await image.Content.ReadAsByteArrayAsync()).ShouldBe(CatalogImageRequests.PngImage);
        image.Content.Headers.ContentType!.MediaType.ShouldBe(CatalogImageRequests.PngContentType);
    }
}
