using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockMovement_initial_stock_is_given;

public sealed class Then_each_size_with_units_gets_a_restock
{
    [Fact]
    public void Then_each_size_with_units_gets_a_restock_Run()
    {
        var product = ProductTestData.Product(StockMode.Tracked, "S", "M", "L");

        var movements = StockMovement.InitialStock(
            product, [new VariantChange(null, "S", InitialStock: 3), new VariantChange(null, "M", InitialStock: 0), new VariantChange(null, "L", InitialStock: 2)], null, TestData.Now);

        movements.Value!.Select(movement => (movement.ProductVariantId, movement.Kind, movement.Quantity)).ShouldBe(
        [
            (product.Variants[0].Id, StockMovementKind.Restock, 3),
            (product.Variants[2].Id, StockMovementKind.Restock, 2),
        ]);
    }
}
