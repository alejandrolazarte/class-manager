using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_is_requested;

public sealed class Then_it_waits_for_payment
{
    [Fact]
    public void Then_it_waits_for_payment_Run()
    {
        var order = OrderTestData.Requested(TestData.Now, OrderTestData.ProductLine());

        order.PaymentStatus.ShouldBe(OrderPaymentStatus.Unpaid);
        order.Balance.ShouldBe(24m);
        order.Status.ShouldBe(OrderStatus.Pending);
    }
}
