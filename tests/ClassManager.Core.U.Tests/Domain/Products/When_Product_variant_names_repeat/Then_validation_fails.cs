using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_variant_names_repeat;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var product = Product.Create(
            ProductTestData.ProductName, null, 12m, StockMode.Tracked, true, [new VariantChange(null, "M"), new VariantChange(null, "m")], TestData.Now);

        product.Error!.FieldName.ShouldBe(Product.VariantsFieldName);
    }
}
