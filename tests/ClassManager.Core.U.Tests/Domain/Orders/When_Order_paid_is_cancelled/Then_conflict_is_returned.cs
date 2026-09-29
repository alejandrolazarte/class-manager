using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_paid_is_cancelled;

public sealed class Then_conflict_is_returned
{
    [Fact]
    public void Then_conflict_is_returned_Run()
    {
        var order = OrderTestData.CounterSale(true, OrderTestData.ProductLine());

        var cancellation = order.Cancel(null, TestData.Now);

        cancellation.Error!.Code.ShouldBe(OrderErrorCodes.NotRequested);
    }
}
