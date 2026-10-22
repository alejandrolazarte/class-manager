using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_variant_has_a_color;

public sealed class Then_its_sale_name_includes_the_color_name
{
    [Fact]
    public void Then_its_sale_name_includes_the_color_name_Run()
    {
        var product = ClassManager.Core.Domain.Products.Product.Create(
            "Remera", null, 12m, StockMode.Tracked, true, [new VariantChange(null, "M", ProductColors.Black)], TestData.Now).Value!.WithId();

        product.SaleNameOf(product.Variants[0]).ShouldBe("Remera · M · Negro");
    }
}
