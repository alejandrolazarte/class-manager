using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_is_sold_at_the_counter_without_paying;

public sealed class Then_nothing_is_paid_yet
{
    [Fact]
    public void Then_nothing_is_paid_yet_Run()
    {
        var order = OrderTestData.PartlyPaidCounterSale(0m, true, OrderTestData.ProductLine());

        order.PaymentStatus.ShouldBe(OrderPaymentStatus.Unpaid);
        order.Payments.ShouldBeEmpty();
        order.IsExpired(TestData.Now.AddDays(30)).ShouldBeFalse();
    }
}
