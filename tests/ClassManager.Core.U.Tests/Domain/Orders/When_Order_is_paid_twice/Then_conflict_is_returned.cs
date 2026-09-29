using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_is_paid_twice;

public sealed class Then_conflict_is_returned
{
    [Fact]
    public void Then_conflict_is_returned_Run()
    {
        var order = OrderTestData.Requested(TestData.Now, OrderTestData.ProductLine());
        order.ConfirmPayment(PaymentMethod.Cash, TestData.Today, TestData.Today, null);

        var payment = order.ConfirmPayment(PaymentMethod.Cash, TestData.Today, TestData.Today, null);

        payment.Error!.Code.ShouldBe(OrderErrorCodes.NotRequested);
    }
}
