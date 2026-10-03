using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_viewer_uploads_a_product_image;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.CreateProductAsync();
        var viewer = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.Viewer);

        using var response = await viewer.PutProductImageAsync(product.Id, CatalogImageRequests.PngImage);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
