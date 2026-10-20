using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_is_handed_over_before_it_is_paid;

public sealed class Then_it_still_owes_the_total
{
    [Fact]
    public void Then_it_still_owes_the_total_Run()
    {
        var order = OrderTestData.Requested(TestData.Now, OrderTestData.ProductLine());

        order.MarkDelivered(null, TestData.Now);

        order.Status.ShouldBe(OrderStatus.Delivered);
        order.Balance.ShouldBe(24m);
    }
}
