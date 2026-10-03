using ClassManager.Core.Common;
using ClassManager.Storage.Images;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Businesses;

public sealed class BrandLogo : ITenantOwned
{
    public const int MaximumSizeInBytes = 512 * 1024;
    public const int ContentTypeMaxLength = 32;
    public const string PngContentType = ImageFormat.PngContentType;

    private const string UnsupportedLogoMessage = "The logo must be a PNG, JPG or WebP image.";
    private const string LogoTooLargeMessage = "The logo must be 512 KB or smaller.";

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
        var contentType = ImageFormat.Detect(content)?.ContentType;
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
}
