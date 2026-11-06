using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_paid_online_is_more_than_its_balance;

public sealed class Then_the_payment_is_refused
{
    [Fact]
    public void Then_the_payment_is_refused_Run()
    {
        var order = OrderTestData.Requested(TestData.Now, OrderTestData.ProductLine());

        var payment = order.RecordOnlinePayment(order.Balance + 1m, "pi_test", TestData.Today, TestData.Now);

        payment.Error!.Code.ShouldBe(OrderErrorCodes.AmountOverBalance);
    }
}
