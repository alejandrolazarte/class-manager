using ClassManager.Core.UseCases.Products;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_owner_removes_the_product_image;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_image_is_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_image_is_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();
        var imageUrl = (await business.HttpClient.SetProductImageAsync(product.Id)).ImageUrl!;

        using var response = await business.HttpClient.DeleteProductImageAsync(product.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<ProductResponse>(ApiRequests.JsonOptions))!.ImageUrl.ShouldBeNull();
        using var removedImage = await fixture.AnonymousClient.GetAsync(new Uri(imageUrl));
        removedImage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
