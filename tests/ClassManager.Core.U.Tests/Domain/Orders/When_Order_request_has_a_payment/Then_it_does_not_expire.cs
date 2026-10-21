namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_request_has_a_payment;

public sealed class Then_it_does_not_expire
{
    [Fact]
    public void Then_it_does_not_expire_Run()
    {
        var order = OrderTestData.Requested(TestData.Now.AddDays(-8), OrderTestData.ProductLine());

        OrderTestData.Pay(order, 10m);

        order.IsExpired(TestData.Now).ShouldBeFalse();
    }
}
