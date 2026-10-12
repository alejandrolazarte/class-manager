using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_color_is_unknown;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var product = ClassManager.Core.Domain.Products.Product.Create(
            ProductTestData.ProductName, null, 12m, StockMode.Tracked, true, [new VariantChange(null, string.Empty, "glitter")], TestData.Now);

        product.Error!.FieldName.ShouldBe(ClassManager.Core.Domain.Products.Product.VariantsFieldName);
    }
}
