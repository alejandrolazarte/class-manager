using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Businesses;

public sealed class BrandLogo : ITenantOwned
{
    public const int MaximumSizeInBytes = 512 * 1024;
    public const int ContentTypeMaxLength = 32;
    public const string PngContentType = "image/png";
    public const string JpegContentType = "image/jpeg";
    public const string WebpContentType = "image/webp";

    private const string UnsupportedLogoMessage = "The logo must be a PNG, JPG or WebP image.";
    private const string LogoTooLargeMessage = "The logo must be 512 KB or smaller.";
    private const int WebpSignatureOffset = 8;

    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];
    private static readonly byte[] RiffSignature = "RIFF"u8.ToArray();
    private static readonly byte[] WebpSignature = "WEBP"u8.ToArray();

    private BrandLogo()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public byte[] Content { get; private set; } = [];
    public string ContentType { get; private set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Result<BrandLogo> Create(byte[] content, DateTimeOffset updatedAt)
    {
        var contentType = DetectContentType(content);
        if (contentType is null)
        {
            return Result.Validation<BrandLogo>(UnsupportedLogoMessage, BrandErrorCodes.UnsupportedLogo, nameof(Content));
        }

        if (content.Length > MaximumSizeInBytes)
        {
            return Result.Validation<BrandLogo>(LogoTooLargeMessage, BrandErrorCodes.LogoTooLarge, nameof(Content));
        }

        return new BrandLogo
        {
            Id = Guid.CreateVersion7(),
            Content = content,
            ContentType = contentType,
            UpdatedAt = updatedAt.ToUniversalTime(),
        };
    }

    public void Replace(BrandLogo newLogo)
    {
        Content = newLogo.Content;
        ContentType = newLogo.ContentType;
        UpdatedAt = newLogo.UpdatedAt;
    }

    private static string? DetectContentType(ReadOnlySpan<byte> content)
    {
        if (content.StartsWith(PngSignature))
        {
            return PngContentType;
        }

        if (content.StartsWith(JpegSignature))
        {
            return JpegContentType;
        }

        return content.StartsWith(RiffSignature) && content.Length > WebpSignatureOffset + WebpSignature.Length
            && content[WebpSignatureOffset..].StartsWith(WebpSignature)
            ? WebpContentType
            : null;
    }
}
