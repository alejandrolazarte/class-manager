using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockRules_product_allows_backorder;

public sealed class Then_any_quantity_can_be_ordered
{
    [Fact]
    public void Then_any_quantity_can_be_ordered_Run()
    {
        var product = ProductTestData.Product(StockMode.TrackedWithBackorder);

        StockRules.MaximumUnitsOf(product, 0).ShouldBeNull();
    }
}
