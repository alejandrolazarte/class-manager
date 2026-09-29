using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products.When_Product_update_leaves_a_variant_out;

public sealed class Then_it_is_deactivated
{
    [Fact]
    public void Then_it_is_deactivated_Run()
    {
        var product = ProductTestData.Product(StockMode.Tracked, "S", "M");
        var small = product.Variants[0];
        var medium = product.Variants[1];

        product.Update(product.Name, null, 12m, StockMode.Tracked, true, [new VariantChange(medium.Id, "M"), new VariantChange(null, "L")]);

        small.IsActive.ShouldBeFalse();
        product.Variants.Count(variant => variant.IsActive).ShouldBe(2);
    }
}
