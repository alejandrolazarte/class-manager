using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_payment_of_a_cancelled_order_is_deleted;

public sealed class Then_conflict_is_returned
{
    [Fact]
    public void Then_conflict_is_returned_Run()
    {
        var order = OrderTestData.PartlyPaidCounterSale(10m, false, OrderTestData.ProductLine());
        order.Cancel(null, TestData.Now);

        var deletion = order.DeletePayment(order.Payments[0].Id);

        deletion.Error!.Code.ShouldBe(OrderErrorCodes.PaymentLocked);
        order.ReturnedAmount.ShouldBe(10m);
    }
}
