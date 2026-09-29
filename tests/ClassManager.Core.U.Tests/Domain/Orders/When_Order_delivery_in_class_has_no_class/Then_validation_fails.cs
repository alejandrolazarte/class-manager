using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.U.Tests.Domain.Orders.When_Order_delivery_in_class_has_no_class;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var order = OrderTestData.Requested(TestData.Now, OrderTestData.ProductLine());

        var delivery = order.ChooseDelivery(DeliveryMethod.InClass, null);

        delivery.Error!.Code.ShouldBe(OrderErrorCodes.ClassNotValid);
    }
}
