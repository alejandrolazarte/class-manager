namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_request_is_seven_days_old;

public sealed class Then_it_is_expired
{
    [Fact]
    public void Then_it_is_expired_Run()
    {
        var order = OrderTestData.Requested(TestData.Now.AddDays(-7), OrderTestData.ProductLine());

        order.IsExpired(TestData.Now).ShouldBeTrue();
        order.IsExpired(TestData.Now.AddMinutes(-1)).ShouldBeFalse();
    }
}
