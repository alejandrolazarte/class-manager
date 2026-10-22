using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_is_handed_over_twice;

public sealed class Then_conflict_is_returned
{
    [Fact]
    public void Then_conflict_is_returned_Run()
    {
        var order = OrderTestData.CounterSale(true, OrderTestData.ProductLine());

        var delivery = order.MarkDelivered(null, TestData.Now);

        delivery.Error!.Code.ShouldBe(OrderErrorCodes.NotPending);
    }
}
