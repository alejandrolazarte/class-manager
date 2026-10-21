namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_payment_deleted_is_restored;

public sealed class Then_it_counts_again
{
    [Fact]
    public void Then_it_counts_again_Run()
    {
        var order = OrderTestData.PartlyPaidCounterSale(10m, false, OrderTestData.ProductLine());
        var payment = order.DeletePayment(order.Payments[0].Id).Value!;
        payment.Delete(TestData.Now);

        var restoration = order.RestorePayment(payment);

        restoration.IsSuccess.ShouldBeTrue();
        (order.PaidAmount, payment.IsDeleted).ShouldBe((10m, false));
    }
}
