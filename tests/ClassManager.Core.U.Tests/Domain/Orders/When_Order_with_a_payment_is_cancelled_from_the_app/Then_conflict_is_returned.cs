using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_with_a_payment_is_cancelled_from_the_app;

public sealed class Then_conflict_is_returned
{
    [Fact]
    public void Then_conflict_is_returned_Run()
    {
        var order = OrderTestData.Requested(TestData.Now, OrderTestData.ProductLine());
        OrderTestData.Pay(order, 10m);

        var cancellation = order.CancelUnpaid(null, TestData.Now);

        cancellation.Error!.Code.ShouldBe(OrderErrorCodes.NotCancellable);
        order.Status.ShouldBe(OrderStatus.Pending);
    }
}
