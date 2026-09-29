using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockRules_tracked_product_has_less_stock;

public sealed class Then_it_cannot_be_sold
{
    [Fact]
    public void Then_it_cannot_be_sold_Run()
    {
        var product = ProductTestData.Product(StockMode.Tracked);

        StockRules.CanSell(product, 1, 2).ShouldBeFalse();
    }
}
