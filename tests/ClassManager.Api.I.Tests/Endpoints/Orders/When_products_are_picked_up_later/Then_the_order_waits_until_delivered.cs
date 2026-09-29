using ClassManager.Core.Domain.Orders;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_products_are_picked_up_later;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_order_waits_until_delivered(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_order_waits_until_delivered_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.RestockAsync(await business.HttpClient.CreateProductAsync(), 3);
        var order = await business.HttpClient.SellAtCounterAsync(
            null, [OrderRequests.ProductLine(product.Variants[0].Id, 1)], isDelivered: false);
        (await business.HttpClient.ListOrdersAsync(awaitingPickup: true)).Single().Id.ShouldBe(order.Id);

        using var response = await business.HttpClient.PutDeliveredAsync(order.Id);

        (await response.Content.ReadFromJsonAsync<OrderResponse>(ApiRequests.JsonOptions))!.Status.ShouldBe(OrderStatus.Delivered);
        (await business.HttpClient.ListOrdersAsync(awaitingPickup: true)).ShouldBeEmpty();
    }
}
