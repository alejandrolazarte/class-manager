namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_owner_replaces_the_product_image;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_previous_image_is_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_previous_image_is_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();
        var firstImageUrl = (await business.HttpClient.SetProductImageAsync(product.Id)).ImageUrl!;

        var secondImageUrl = (await business.HttpClient.SetProductImageAsync(product.Id)).ImageUrl!;

        secondImageUrl.ShouldNotBe(firstImageUrl);
        using var previousImage = await fixture.AnonymousClient.GetAsync(new Uri(firstImageUrl));
        previousImage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
