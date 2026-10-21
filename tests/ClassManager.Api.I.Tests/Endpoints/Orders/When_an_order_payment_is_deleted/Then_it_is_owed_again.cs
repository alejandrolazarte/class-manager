using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_an_order_payment_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_owed_again(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_owed_again_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var product = await owner.CreateProductAsync(StockMode.Unlimited);
        var order = await owner.SellAtCounterAsync(null, [OrderRequests.ProductLine(product.Variants[0].Id, 1)]);

        using var response = await owner.DeleteOrderPaymentAsync(order.Id, order.Payments.Single().Id);

        var updatedOrder = (await response.Content.ReadFromJsonAsync<OrderResponse>(ApiRequests.JsonOptions))!;
        (updatedOrder.PaymentStatus, updatedOrder.Balance).ShouldBe((OrderPaymentStatus.Unpaid, order.Total));
        (await owner.GetOrderSummaryAsync()).Collected.ShouldBe(0m);
    }
}
