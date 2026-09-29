namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_refunds_half_the_classes;

public sealed class Then_half_the_price_is_refunded
{
    [Fact]
    public void Then_half_the_price_is_refunded_Run()
    {
        var line = OrderTestData.PackLine();
        var order = OrderTestData.CounterSale(true, line);

        var refund = order.RefundUnusedClasses(line, 4, 2);

        refund.Value.ShouldBe(40m);
        order.RefundedAmount.ShouldBe(40m);
    }
}
