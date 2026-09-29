using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_refunding_a_product_returned_to_the_shelf;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_stock_goes_back(ApiFixture fixture)
{
    [Fact]
    public async Task Then_stock_goes_back_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.RestockAsync(await business.HttpClient.CreateProductAsync(), 3);
        var order = await business.HttpClient.SellAtCounterAsync(null, [OrderRequests.ProductLine(product.Variants[0].Id, 2)]);

        using var response = await business.HttpClient.PostRefundAsync(order.Id, new RefundLine(order.Lines[0].Id, 1, true));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await business.HttpClient.ListProductsAsync()).Single().Variants[0].Stock.ShouldBe(2);
    }
}
