using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_returns_more_units_than_sold;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var line = OrderTestData.ProductLine(quantity: 2);
        var order = OrderTestData.CounterSale(true, line);
        order.RefundProductUnits(line, 1);

        var refund = order.RefundProductUnits(line, 2);

        refund.Error!.Code.ShouldBe(OrderErrorCodes.NothingToRefund);
    }
}
