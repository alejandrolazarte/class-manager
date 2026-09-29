using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_sells_a_pack_without_client;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var order = Order.CounterSale(
            null, [OrderTestData.PackLine()], PaymentMethod.Cash, TestData.Today, null, true, TestData.Today, null, TestData.Now);

        order.Error!.Code.ShouldBe(OrderErrorCodes.ClientRequired);
    }
}
