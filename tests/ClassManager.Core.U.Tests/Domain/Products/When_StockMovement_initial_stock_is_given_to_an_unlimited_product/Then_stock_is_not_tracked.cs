using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockMovement_initial_stock_is_given_to_an_unlimited_product;

public sealed class Then_stock_is_not_tracked
{
    [Fact]
    public void Then_stock_is_not_tracked_Run()
    {
        var product = ProductTestData.Product(StockMode.Unlimited);

        var movements = StockMovement.InitialStock(product, [new VariantChange(null, string.Empty, InitialStock: 5)], null, TestData.Now);

        movements.Error!.Code.ShouldBe(ProductErrorCodes.StockNotTracked);
    }
}
