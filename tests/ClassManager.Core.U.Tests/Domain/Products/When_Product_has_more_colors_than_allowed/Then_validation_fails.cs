using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_has_more_colors_than_allowed;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var colors = ProductColors.Keys.Take(ClassManager.Core.Domain.Products.Product.MaximumColorCount + 1);

        var product = ClassManager.Core.Domain.Products.Product.Create(
            ProductTestData.ProductName, null, 12m, StockMode.Tracked, true, [.. colors.Select(color => new VariantChange(null, string.Empty, color))], TestData.Now);

        product.Error!.FieldName.ShouldBe(ClassManager.Core.Domain.Products.Product.VariantsFieldName);
    }
}
