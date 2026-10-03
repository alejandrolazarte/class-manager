namespace ClassManager.Storage.Images;

public sealed record ImageFormat(string ContentType, string FileExtension)
{
    public const string PngContentType = "image/png";
    public const string JpegContentType = "image/jpeg";
    public const string WebpContentType = "image/webp";

    private const int WebpSignatureOffset = 8;

    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];
    private static readonly byte[] RiffSignature = "RIFF"u8.ToArray();
    private static readonly byte[] WebpSignature = "WEBP"u8.ToArray();

    public static ImageFormat Png { get; } = new(PngContentType, ".png");

    public static ImageFormat Jpeg { get; } = new(JpegContentType, ".jpg");

    public static ImageFormat Webp { get; } = new(WebpContentType, ".webp");

    public static ImageFormat? Detect(ReadOnlySpan<byte> content)
    {
        if (content.StartsWith(PngSignature))
        {
            return Png;
        }

        if (content.StartsWith(JpegSignature))
        {
            return Jpeg;
        }

        return content.StartsWith(RiffSignature)
            && content.Length >= WebpSignatureOffset + WebpSignature.Length
            && content[WebpSignatureOffset..].StartsWith(WebpSignature)
            ? Webp
            : null;
    }
}
