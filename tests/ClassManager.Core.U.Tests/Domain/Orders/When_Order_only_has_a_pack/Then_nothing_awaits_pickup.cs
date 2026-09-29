using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_only_has_a_pack;

public sealed class Then_nothing_awaits_pickup
{
    [Fact]
    public void Then_nothing_awaits_pickup_Run()
    {
        var order = OrderTestData.CounterSale(false, OrderTestData.PackLine());

        order.MarkDelivered(null, TestData.Now).Error!.Code.ShouldBe(OrderErrorCodes.NotAwaitingPickup);
    }
}
