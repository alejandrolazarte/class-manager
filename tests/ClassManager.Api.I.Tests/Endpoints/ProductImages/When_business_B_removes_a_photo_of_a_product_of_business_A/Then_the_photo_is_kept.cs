namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_business_B_removes_a_photo_of_a_product_of_business_A;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_photo_is_kept(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_photo_is_kept_Run()
    {
        var businessA = await fixture.SeedBusinessAsync();
        var businessB = await fixture.SeedBusinessAsync();
        var productOfA = await businessA.HttpClient.CreateProductAsync();
        var imageOfA = (await businessA.HttpClient.AddProductImageAsync(productOfA.Id)).Images.Single();

        using var response = await businessB.HttpClient.DeleteProductImageAsync(productOfA.Id, imageOfA.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        using var image = await fixture.AnonymousClient.GetAsync(new Uri(imageOfA.Url));
        image.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
