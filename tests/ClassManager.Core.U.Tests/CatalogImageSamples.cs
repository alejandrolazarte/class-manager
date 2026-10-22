using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.U.Tests;

internal static class CatalogImageSamples
{
    public static readonly byte[] Png = BrandLogoSamples.Png;

    public static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46];

    public static readonly byte[] PlainText = "not an image"u8.ToArray();

    public static Document PublicDocument() =>
        Document.Create(
            $"tenant/products/{TestIds.Next()}.png",
            new DocumentContent(Png, "image/png", ".png"),
            DocumentVisibility.Public,
            TestData.Now).WithId();

    public static Product ProductWithImages(int imageCount)
    {
        var product = Product.Create("Gorro", null, 12m, StockMode.Unlimited, true, [new VariantChange(null, null)], TestData.Now).Value!.WithId();
        for (var index = 0; index < imageCount; index++)
        {
            product.AddImage(PublicDocument());
        }

        return product;
    }

    public static ClassPack ClassPackWithImages(int imageCount)
    {
        var classPack = ClassPack.Create("4 clases", 4, 80m, null, TestData.Now).Value!.WithId();
        for (var index = 0; index < imageCount; index++)
        {
            classPack.AddImage(PublicDocument());
        }

        return classPack;
    }
}
