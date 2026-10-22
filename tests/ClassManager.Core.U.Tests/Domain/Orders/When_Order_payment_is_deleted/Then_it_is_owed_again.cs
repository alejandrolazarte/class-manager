using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_payment_is_deleted;

public sealed class Then_it_is_owed_again
{
    [Fact]
    public void Then_it_is_owed_again_Run()
    {
        var order = OrderTestData.CounterSale(true, OrderTestData.ProductLine());

        var deletion = order.DeletePayment(order.Payments[0].Id);

        deletion.IsSuccess.ShouldBeTrue();
        (order.Balance, order.PaymentStatus).ShouldBe((24m, OrderPaymentStatus.Unpaid));
    }
}
