namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_is_numbered;

public sealed class Then_it_keeps_the_number
{
    private const int OrderNumber = 1043;

    [Fact]
    public void Then_it_keeps_the_number_Run()
    {
        var order = OrderTestData.CounterSale(isDelivered: true, OrderTestData.PackLine());

        order.AssignNumber(OrderNumber);

        order.Number.ShouldBe(OrderNumber);
    }
}
