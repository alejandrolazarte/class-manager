using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_update_names_a_variant_of_another_product;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var product = ProductTestData.Product();
        var otherProduct = ProductTestData.Product();

        var update = product.Update(
            product.Name, null, 12m, StockMode.Tracked, true, [new VariantChange(otherProduct.Variants[0].Id, string.Empty)]);

        update.Error!.FieldName.ShouldBe(Product.VariantsFieldName);
    }
}
