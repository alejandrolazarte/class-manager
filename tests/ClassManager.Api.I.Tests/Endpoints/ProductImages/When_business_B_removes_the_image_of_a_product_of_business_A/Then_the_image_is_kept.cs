namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_business_B_removes_the_image_of_a_product_of_business_A;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_image_is_kept(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_image_is_kept_Run()
    {
        var businessA = await fixture.SeedBusinessAsync();
        var businessB = await fixture.SeedBusinessAsync();
        var productOfA = await businessA.HttpClient.CreateProductAsync();
        var imageUrl = (await businessA.HttpClient.SetProductImageAsync(productOfA.Id)).ImageUrl!;

        using var response = await businessB.HttpClient.DeleteProductImageAsync(productOfA.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        using var image = await fixture.AnonymousClient.GetAsync(new Uri(imageUrl));
        image.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
