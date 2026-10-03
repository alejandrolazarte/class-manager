namespace ClassManager.Core.U.Tests;

internal static class CatalogImageSamples
{
    public const string PreviousImageUrl = "https://files.example.com/public-files/tenant/products/previous.png";

    public static readonly byte[] Png = BrandLogoSamples.Png;

    public static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46];

    public static readonly byte[] PlainText = "not an image"u8.ToArray();
}
