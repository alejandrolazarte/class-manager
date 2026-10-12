using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_update_removes_a_color;

public sealed class Then_its_variants_are_deactivated
{
    [Fact]
    public void Then_its_variants_are_deactivated_Run()
    {
        var product = ClassManager.Core.Domain.Products.Product.Create(
            ProductTestData.ProductName,
            null,
            12m,
            StockMode.Tracked,
            true,
            [new VariantChange(null, "S", ProductColors.Black), new VariantChange(null, "S", ProductColors.White)],
            TestData.Now).Value!;
        var black = product.Variants[0];
        var white = product.Variants[1];

        product.Update(product.Name, null, 12m, StockMode.Tracked, true, [new VariantChange(black.Id, "S", ProductColors.Black)]);

        white.IsActive.ShouldBeFalse();
        black.IsActive.ShouldBeTrue();
    }
}
