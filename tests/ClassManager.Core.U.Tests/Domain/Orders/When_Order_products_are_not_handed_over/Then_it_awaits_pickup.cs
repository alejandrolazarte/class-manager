using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_products_are_not_handed_over;

public sealed class Then_it_awaits_pickup
{
    [Fact]
    public void Then_it_awaits_pickup_Run()
    {
        var order = OrderTestData.CounterSale(false, OrderTestData.ProductLine());

        order.AwaitsPickup.ShouldBeTrue();
        order.Status.ShouldBe(OrderStatus.Paid);
    }
}
