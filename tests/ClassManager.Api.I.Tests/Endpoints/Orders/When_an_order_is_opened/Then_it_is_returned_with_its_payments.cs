using ClassManager.Core.Domain.Products;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_an_order_is_opened;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_returned_with_its_payments(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_returned_with_its_payments_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var product = await owner.CreateProductAsync(StockMode.Unlimited);
        var order = await owner.SellAtCounterAsync(null, [OrderRequests.ProductLine(product.Variants[0].Id, 1)], isDelivered: false, paidAmount: 5m);

        var openedOrder = await owner.GetOrderAsync(order.Id);

        (openedOrder.Id, openedOrder.PaidAmount, openedOrder.Payments.Count).ShouldBe((order.Id, 5m, 1));
    }
}
