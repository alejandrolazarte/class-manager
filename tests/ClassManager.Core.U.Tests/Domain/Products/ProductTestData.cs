using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Products;

internal static class ProductTestData
{
    public const string ProductName = "Gorro de natación";

    public static Product Product(StockMode stockMode = StockMode.Tracked, params string[] variantNames) =>
        ClassManager.Core.Domain.Products.Product.Create(
            ProductName,
            null,
            12m,
            stockMode,
            true,
            variantNames.Length == 0 ? [new VariantChange(null, string.Empty)] : [.. variantNames.Select(name => new VariantChange(null, name))],
            TestData.Now).Value!;
}
