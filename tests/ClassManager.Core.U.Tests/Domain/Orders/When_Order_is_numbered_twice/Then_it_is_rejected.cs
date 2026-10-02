namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_is_numbered_twice;

public sealed class Then_it_is_rejected
{
    [Fact]
    public void Then_it_is_rejected_Run()
    {
        var order = OrderTestData.CounterSale(isDelivered: true, OrderTestData.PackLine());
        order.AssignNumber(1);

        Should.Throw<InvalidOperationException>(() => order.AssignNumber(2));
    }
}
