namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_owner_adds_a_product_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_served_from_its_url(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_served_from_its_url_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();

        var updatedProduct = await business.HttpClient.AddProductImageAsync(product.Id);

        using var image = await fixture.AnonymousClient.GetAsync(new Uri(updatedProduct.Images.Single().Url));
        (await image.Content.ReadAsByteArrayAsync()).ShouldBe(CatalogImageRequests.PngImage);
        image.Content.Headers.ContentType!.MediaType.ShouldBe(CatalogImageRequests.PngContentType);
    }
}
