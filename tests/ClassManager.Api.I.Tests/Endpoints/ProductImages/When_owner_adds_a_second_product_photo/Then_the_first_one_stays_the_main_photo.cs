namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_owner_adds_a_second_product_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_first_one_stays_the_main_photo(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_first_one_stays_the_main_photo_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();
        var firstImage = (await business.HttpClient.AddProductImageAsync(product.Id)).Images.Single();

        await business.HttpClient.AddProductImageAsync(product.Id);

        var listedImages = (await business.HttpClient.ListProductsAsync()).Single().Images;
        listedImages.Count.ShouldBe(2);
        listedImages[0].ShouldBe(firstImage);
    }
}
