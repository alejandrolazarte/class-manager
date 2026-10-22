using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockRules_tracked_product_has_stock;

public sealed class Then_its_stock_is_the_most_that_can_be_ordered
{
    [Fact]
    public void Then_its_stock_is_the_most_that_can_be_ordered_Run()
    {
        var product = ProductTestData.Product(StockMode.Tracked);

        StockRules.MaximumUnitsOf(product, 3).ShouldBe(3);
    }
}
