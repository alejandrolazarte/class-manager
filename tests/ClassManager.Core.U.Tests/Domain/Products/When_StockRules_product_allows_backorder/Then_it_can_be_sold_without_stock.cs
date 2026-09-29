using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockRules_product_allows_backorder;

public sealed class Then_it_can_be_sold_without_stock
{
    [Fact]
    public void Then_it_can_be_sold_without_stock_Run()
    {
        var product = ProductTestData.Product(StockMode.TrackedWithBackorder);

        StockRules.CanSell(product, 0, 2).ShouldBeTrue();
    }
}
