using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_has_sizes_and_colors;

public sealed class Then_each_combination_is_a_variant
{
    [Fact]
    public void Then_each_combination_is_a_variant_Run()
    {
        var product = ClassManager.Core.Domain.Products.Product.Create(
            ProductTestData.ProductName,
            null,
            12m,
            StockMode.Tracked,
            true,
            [
                new VariantChange(null, "S", ProductColors.Black),
                new VariantChange(null, "S", ProductColors.White),
                new VariantChange(null, "M", ProductColors.Black),
                new VariantChange(null, "M", ProductColors.White),
            ],
            TestData.Now).Value!;

        product.Variants.Select(variant => (variant.Size, variant.Color)).ShouldBe(
        [
            ("S", ProductColors.Black),
            ("S", ProductColors.White),
            ("M", ProductColors.Black),
            ("M", ProductColors.White),
        ]);
    }
}
