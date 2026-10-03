using ClassManager.Core.UseCases.Products;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_owner_removes_a_product_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_file_is_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_file_is_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();
        var image = (await business.HttpClient.AddProductImageAsync(product.Id)).Images.Single();

        using var response = await business.HttpClient.DeleteProductImageAsync(product.Id, image.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<ProductResponse>(ApiRequests.JsonOptions))!.Images.ShouldBeEmpty();
        using var removedImage = await fixture.AnonymousClient.GetAsync(new Uri(image.Url));
        removedImage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
