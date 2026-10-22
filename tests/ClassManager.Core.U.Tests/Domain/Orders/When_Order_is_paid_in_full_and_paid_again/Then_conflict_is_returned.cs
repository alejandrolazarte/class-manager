using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_is_paid_in_full_and_paid_again;

public sealed class Then_conflict_is_returned
{
    [Fact]
    public void Then_conflict_is_returned_Run()
    {
        var order = OrderTestData.Requested(TestData.Now, OrderTestData.ProductLine());
        OrderTestData.Pay(order, 24m);

        var payment = OrderTestData.Pay(order, 1m);

        payment.Error!.Code.ShouldBe(OrderErrorCodes.NothingOwed);
    }
}
