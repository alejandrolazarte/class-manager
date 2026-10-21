using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_payment_restored_no_longer_fits;

public sealed class Then_conflict_is_returned
{
    [Fact]
    public void Then_conflict_is_returned_Run()
    {
        var order = OrderTestData.PartlyPaidCounterSale(10m, false, OrderTestData.ProductLine());
        var payment = order.DeletePayment(order.Payments[0].Id).Value!;
        payment.Delete(TestData.Now);
        OrderTestData.Pay(order, 24m);

        var restoration = order.RestorePayment(payment);

        restoration.Error!.Code.ShouldBe(OrderErrorCodes.AmountOverBalance);
        order.PaidAmount.ShouldBe(24m);
    }
}
