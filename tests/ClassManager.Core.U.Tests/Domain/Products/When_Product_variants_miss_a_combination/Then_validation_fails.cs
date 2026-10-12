using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_variants_miss_a_combination;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var product = ClassManager.Core.Domain.Products.Product.Create(
            ProductTestData.ProductName,
            null,
            12m,
            StockMode.Tracked,
            true,
            [
                new VariantChange(null, "S", ProductColors.Black),
                new VariantChange(null, "M", ProductColors.Black),
                new VariantChange(null, "S", ProductColors.White),
            ],
            TestData.Now);

        product.Error!.FieldName.ShouldBe(ClassManager.Core.Domain.Products.Product.VariantsFieldName);
    }
}
