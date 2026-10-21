using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_a_counter_sale_is_paid_in_two_parts;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_nothing_is_owed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_nothing_is_owed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var product = await owner.CreateProductAsync(StockMode.Unlimited);
        var order = await owner.SellAtCounterAsync(null, [OrderRequests.ProductLine(product.Variants[0].Id, 2)], paidAmount: 10m);
        (await owner.ListOrdersAsync(owing: true)).Single().Id.ShouldBe(order.Id);

        using var response = await owner.PostOrderPaymentAsync(order.Id);

        var paidOrder = (await response.Content.ReadFromJsonAsync<OrderResponse>(ApiRequests.JsonOptions))!;
        (paidOrder.PaymentStatus, paidOrder.Balance, paidOrder.Payments.Count).ShouldBe((OrderPaymentStatus.Paid, 0m, 2));
        (await owner.ListOrdersAsync(owing: true)).ShouldBeEmpty();
    }
}
