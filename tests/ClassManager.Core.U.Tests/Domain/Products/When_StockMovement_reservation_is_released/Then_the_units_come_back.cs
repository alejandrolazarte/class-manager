using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockMovement_reservation_is_released;

public sealed class Then_the_units_come_back
{
    [Fact]
    public void Then_the_units_come_back_Run()
    {
        var reservation = StockMovement.Reserve(TestIds.Next(), 2, TestIds.Next(), null, TestData.Now).WithId();

        var release = StockMovement.Release(reservation, null, TestData.Now).WithId();

        (reservation.Quantity + release.Quantity).ShouldBe(0);
        release.Kind.ShouldBe(StockMovementKind.Cancellation);
    }
}
