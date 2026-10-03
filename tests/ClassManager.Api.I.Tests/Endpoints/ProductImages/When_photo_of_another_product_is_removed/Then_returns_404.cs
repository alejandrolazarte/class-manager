using ClassManager.Core.UseCases.Products;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_photo_of_another_product_is_removed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var cap = await business.HttpClient.CreateProductAsync();
        using var swimsuitResponse = await business.HttpClient.PostProductAsync("Malla");
        var swimsuit = (await swimsuitResponse.Content.ReadFromJsonAsync<ProductResponse>(ApiRequests.JsonOptions))!;
        var capImage = (await business.HttpClient.AddProductImageAsync(cap.Id)).Images.Single();

        using var response = await business.HttpClient.DeleteProductImageAsync(swimsuit.Id, capImage.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
