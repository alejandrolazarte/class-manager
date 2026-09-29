namespace ClassManager.Api.I.Tests.Endpoints.FamilyShop.When_branch_cancels_a_family_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_stock_is_released(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_stock_is_released_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        var order = await scenario.Family.PlaceFamilyOrderAsync(FamilyShopRequests.ProductLine(product.Variants[0].Id, 2));

        using var response = await owner.PutOrderCancellationAsync(order.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await owner.ListProductsAsync()).Single().Variants[0].Stock.ShouldBe(2);
    }
}
