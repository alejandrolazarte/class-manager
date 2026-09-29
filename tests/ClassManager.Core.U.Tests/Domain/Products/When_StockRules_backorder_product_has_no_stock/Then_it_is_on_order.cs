using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_StockRules_backorder_product_has_no_stock;

public sealed class Then_it_is_on_order
{
    [Fact]
    public void Then_it_is_on_order_Run()
    {
        var product = ProductTestData.Product(StockMode.TrackedWithBackorder);

        StockRules.AvailabilityOf(product, 0).ShouldBe(StockAvailability.OnOrder);
    }
}
