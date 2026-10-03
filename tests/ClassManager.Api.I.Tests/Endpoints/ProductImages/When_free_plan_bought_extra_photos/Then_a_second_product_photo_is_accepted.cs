using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_free_plan_bought_extra_photos;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_second_product_photo_is_accepted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_second_product_photo_is_accepted_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        await fixture.AddFeatureAsync(business, Features.CatalogPhotos, limit: 10);
        var product = await business.HttpClient.CreateProductAsync();
        await business.HttpClient.AddProductImageAsync(product.Id);

        using var response = await business.HttpClient.PostProductImageAsync(product.Id, CatalogImageRequests.PngImage);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
