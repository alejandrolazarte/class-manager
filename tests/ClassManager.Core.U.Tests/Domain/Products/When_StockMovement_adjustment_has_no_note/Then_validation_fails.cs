using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockMovement_adjustment_has_no_note;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var product = ProductTestData.Product();

        var movement = StockMovement.Load(
            product, product.Variants[0], StockMovementKind.Adjustment, -1, " ", 5, null, TestData.Now);

        movement.Error!.FieldName.ShouldBe(nameof(StockMovement.Note));
    }
}
