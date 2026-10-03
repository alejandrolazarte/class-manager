namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_owner_uploads_a_product_image;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_url_is_kept_in_the_product(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_url_is_kept_in_the_product_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();

        var updatedProduct = await business.HttpClient.SetProductImageAsync(product.Id);

        var listedProduct = (await business.HttpClient.ListProductsAsync()).Single();
        listedProduct.ImageUrl.ShouldBe(updatedProduct.ImageUrl);
        listedProduct.ImageUrl!.ShouldContain($"/{business.Business.Id}/products/{product.Id}/");
    }
}
