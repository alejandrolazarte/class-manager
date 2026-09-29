using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockMovement_product_is_unlimited;

public sealed class Then_stock_is_not_tracked
{
    [Fact]
    public void Then_stock_is_not_tracked_Run()
    {
        var product = ProductTestData.Product(StockMode.Unlimited);

        var movement = StockMovement.Load(
            product, product.Variants[0], StockMovementKind.Restock, 5, null, 0, null, TestData.Now);

        movement.Error!.Code.ShouldBe(ProductErrorCodes.StockNotTracked);
    }
}
