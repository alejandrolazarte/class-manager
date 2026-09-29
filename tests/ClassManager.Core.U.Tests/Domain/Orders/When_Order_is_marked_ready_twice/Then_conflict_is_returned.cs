using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_is_marked_ready_twice;

public sealed class Then_conflict_is_returned
{
    [Fact]
    public void Then_conflict_is_returned_Run()
    {
        var order = OrderTestData.CounterSale(false, OrderTestData.ProductLine());
        order.MarkReady(TestData.Now);

        var ready = order.MarkReady(TestData.Now);

        ready.Error!.Code.ShouldBe(OrderErrorCodes.AlreadyReady);
    }
}
