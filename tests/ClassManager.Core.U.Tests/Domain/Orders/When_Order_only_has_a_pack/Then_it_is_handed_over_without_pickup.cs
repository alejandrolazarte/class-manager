using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_only_has_a_pack;

public sealed class Then_it_is_handed_over_without_pickup
{
    [Fact]
    public void Then_it_is_handed_over_without_pickup_Run()
    {
        var order = OrderTestData.CounterSale(false, OrderTestData.PackLine());

        var delivery = order.MarkDelivered(null, TestData.Now);

        delivery.IsSuccess.ShouldBeTrue();
        order.Status.ShouldBe(OrderStatus.Delivered);
        order.AwaitsPickup.ShouldBeFalse();
    }
}
