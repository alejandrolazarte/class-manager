namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_business_B_adds_a_photo_to_a_product_of_business_A;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var businessA = await fixture.SeedBusinessAsync();
        var businessB = await fixture.SeedBusinessAsync();
        var productOfA = await businessA.HttpClient.CreateProductAsync();

        using var response = await businessB.HttpClient.PostProductImageAsync(productOfA.Id, CatalogImageRequests.PngImage);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await businessA.HttpClient.ListProductsAsync()).Single().Images.ShouldBeEmpty();
    }
}
