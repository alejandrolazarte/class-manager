using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_cancelled_is_paid_online;

public sealed class Then_the_payment_is_refused
{
    [Fact]
    public void Then_the_payment_is_refused_Run()
    {
        var order = OrderTestData.Requested(TestData.Now, OrderTestData.ProductLine());
        order.CancelUnpaid(cancelledByUserId: null, TestData.Now);

        var payment = order.RecordOnlinePayment(order.Total, "pi_test", TestData.Today, TestData.Now);

        payment.Error!.Code.ShouldBe(OrderErrorCodes.NothingOwed);
    }
}
