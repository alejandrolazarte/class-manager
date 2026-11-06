namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_is_paid_online;

public sealed class Then_it_can_no_longer_be_paid_online
{
    [Fact]
    public void Then_it_can_no_longer_be_paid_online_Run()
    {
        var order = OrderTestData.Requested(TestData.Now, OrderTestData.ProductLine());

        order.RecordOnlinePayment(order.Balance, "pi_test", TestData.Today, TestData.Now);

        order.CanBePaidOnline.ShouldBeFalse();
    }
}
