using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyShop.When_family_orders_a_product_hidden_from_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        using var created = await scenario.Coaches.Business.HttpClient.PostAsJsonAsync(
            ApiRoutes.Products,
            new CreateProductCommand("Toalla del staff", null, 10m, StockMode.Unlimited, false, null),
            ApiRequests.JsonOptions);
        var hidden = (await created.Content.ReadFromJsonAsync<ProductResponse>(ApiRequests.JsonOptions))!;

        using var response = await scenario.Family.PostFamilyOrderAsync(FamilyShopRequests.ProductLine(hidden.Variants[0].Id, 1));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
