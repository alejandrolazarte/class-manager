using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.ProductImages.When_free_plan_adds_a_second_product_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_plan_limit_is_reached(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_plan_limit_is_reached_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var product = await business.HttpClient.CreateProductAsync();
        await business.HttpClient.AddProductImageAsync(product.Id);

        using var response = await business.HttpClient.PostProductImageAsync(product.Id, CatalogImageRequests.PngImage);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.Content.ReadAsStringAsync()).ShouldContain(FeatureErrorCodes.LimitReached);
    }
}
