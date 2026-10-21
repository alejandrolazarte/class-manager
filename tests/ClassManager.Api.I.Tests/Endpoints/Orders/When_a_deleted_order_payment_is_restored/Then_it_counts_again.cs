using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_a_deleted_order_payment_is_restored;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_counts_again(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_counts_again_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var product = await owner.CreateProductAsync(StockMode.Unlimited);
        var order = await owner.SellAtCounterAsync(null, [OrderRequests.ProductLine(product.Variants[0].Id, 1)]);
        var paymentId = order.Payments.Single().Id;
        (await owner.DeleteOrderPaymentAsync(order.Id, paymentId)).EnsureSuccessStatusCode();

        using var response = await owner.PostOrderPaymentRestoreAsync(order.Id, paymentId);

        var restoredOrder = (await response.Content.ReadFromJsonAsync<OrderResponse>(ApiRequests.JsonOptions))!;
        (restoredOrder.PaymentStatus, restoredOrder.Payments.Single().Id).ShouldBe((OrderPaymentStatus.Paid, paymentId));
        (await owner.GetOrderSummaryAsync()).Collected.ShouldBe(order.Total);
    }
}
