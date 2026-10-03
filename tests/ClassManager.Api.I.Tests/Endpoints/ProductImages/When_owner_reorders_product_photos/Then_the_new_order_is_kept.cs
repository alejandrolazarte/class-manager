namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_owner_reorders_product_photos;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_new_order_is_kept(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_new_order_is_kept_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();
        await business.HttpClient.AddProductImageAsync(product.Id);
        var images = (await business.HttpClient.AddProductImageAsync(product.Id)).Images;

        using var response = await business.HttpClient.PutProductImagesOrderAsync(product.Id, [images[1].Id, images[0].Id]);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var listedImages = (await business.HttpClient.ListProductsAsync()).Single().Images;
        listedImages.Select(image => image.Id).ShouldBe([images[1].Id, images[0].Id]);
    }
}
