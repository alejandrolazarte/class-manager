using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockMovement_adjustment_leaves_stock_negative;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var product = ProductTestData.Product();

        var movement = StockMovement.Load(
            product, product.Variants[0], StockMovementKind.Adjustment, -3, "Roto", 2, null, TestData.Now);

        movement.Error!.FieldName.ShouldBe(nameof(StockMovement.Quantity));
    }
}
