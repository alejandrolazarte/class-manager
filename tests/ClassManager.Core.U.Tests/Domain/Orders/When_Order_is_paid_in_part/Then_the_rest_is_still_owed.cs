using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_is_paid_in_part;

public sealed class Then_the_rest_is_still_owed
{
    [Fact]
    public void Then_the_rest_is_still_owed_Run()
    {
        var order = OrderTestData.PartlyPaidCounterSale(30m, true, OrderTestData.PackLine());

        OrderTestData.Pay(order, 20m);

        order.Balance.ShouldBe(30m);
        order.PaidAmount.ShouldBe(50m);
        order.PaymentStatus.ShouldBe(OrderPaymentStatus.Partial);
    }
}
