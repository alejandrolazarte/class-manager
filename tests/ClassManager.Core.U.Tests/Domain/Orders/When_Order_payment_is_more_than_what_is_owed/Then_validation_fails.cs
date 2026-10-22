using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_payment_is_more_than_what_is_owed;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var order = OrderTestData.PartlyPaidCounterSale(20m, false, OrderTestData.ProductLine());

        var payment = OrderTestData.Pay(order, 5m);

        payment.Error!.Code.ShouldBe(OrderErrorCodes.AmountOverBalance);
        order.PaidAmount.ShouldBe(20m);
    }
}
