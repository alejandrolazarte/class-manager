using ClassManager.Core.Domain.Products;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_a_payment_of_a_cancelled_order_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var product = await owner.CreateProductAsync(StockMode.Unlimited);
        var order = await owner.SellAtCounterAsync(null, [OrderRequests.ProductLine(product.Variants[0].Id, 1)], isDelivered: false, paidAmount: 5m);
        (await owner.PutOrderCancellationAsync(order.Id)).EnsureSuccessStatusCode();

        using var response = await owner.DeleteOrderPaymentAsync(order.Id, order.Payments.Single().Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await owner.ListOrdersAsync()).Single().ReturnedAmount.ShouldBe(5m);
    }
}
