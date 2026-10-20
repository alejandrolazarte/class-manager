namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_partly_paid_returns_units;

public sealed class Then_the_return_lowers_what_is_owed
{
    [Fact]
    public void Then_the_return_lowers_what_is_owed_Run()
    {
        var line = OrderTestData.ProductLine();
        var order = OrderTestData.PartlyPaidCounterSale(6m, true, line);

        order.RefundProductUnits(line, 1);

        order.Balance.ShouldBe(6m);
        order.CollectedAmount.ShouldBe(6m);
    }
}
