using ClassManager.Core.Domain.Orders;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_branch_cancels_an_order_paid_in_part;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_payment_is_returned_and_the_stock_restored(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_payment_is_returned_and_the_stock_restored_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        var order = await owner.SellAtCounterAsync(null, [OrderRequests.ProductLine(product.Variants[0].Id, 1)], isDelivered: false, paidAmount: 5m);

        using var response = await owner.PutOrderCancellationAsync(order.Id);

        var cancelledOrder = (await response.Content.ReadFromJsonAsync<OrderResponse>(ApiRequests.JsonOptions))!;
        (cancelledOrder.Status, cancelledOrder.ReturnedAmount, cancelledOrder.Balance).ShouldBe((OrderStatus.Cancelled, 5m, 0m));
        (await owner.GetOrderSummaryAsync()).Collected.ShouldBe(0m);
        (await owner.ListProductsAsync()).Single().Variants[0].Stock.ShouldBe(2);
    }
}
