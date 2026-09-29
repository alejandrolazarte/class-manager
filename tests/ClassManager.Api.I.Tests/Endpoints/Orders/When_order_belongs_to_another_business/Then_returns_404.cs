namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_order_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var product = await otherBusiness.HttpClient.RestockAsync(await otherBusiness.HttpClient.CreateProductAsync(), 1);
        var otherOrder = await otherBusiness.HttpClient.SellAtCounterAsync(
            null, [OrderRequests.ProductLine(product.Variants[0].Id, 1)], isDelivered: false);

        using var response = await business.HttpClient.PutDeliveredAsync(otherOrder.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
