using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_with_a_payment_is_cancelled;

public sealed class Then_what_was_paid_is_returned
{
    [Fact]
    public void Then_what_was_paid_is_returned_Run()
    {
        var order = OrderTestData.PartlyPaidCounterSale(10m, false, OrderTestData.ProductLine());

        order.Cancel(null, TestData.Now);

        order.Status.ShouldBe(OrderStatus.Cancelled);
        (order.ReturnedAmount, order.CollectedAmount, order.Balance).ShouldBe((10m, 0m, 0m));
    }
}
