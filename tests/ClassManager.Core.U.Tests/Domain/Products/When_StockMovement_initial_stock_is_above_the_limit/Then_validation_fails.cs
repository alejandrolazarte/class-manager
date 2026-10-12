using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockMovement_initial_stock_is_above_the_limit;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var product = ProductTestData.Product();

        var movements = StockMovement.InitialStock(
            product, [new VariantChange(null, string.Empty, InitialStock: StockMovement.MaximumQuantity + 1)], null, TestData.Now);

        movements.Error!.FieldName.ShouldBe(Product.VariantsFieldName);
    }
}
