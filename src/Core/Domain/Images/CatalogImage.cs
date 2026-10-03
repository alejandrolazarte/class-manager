using ClassManager.Core.Common;
using ClassManager.Storage.Images;

namespace ClassManager.Core.Domain.Images;

public sealed class CatalogImage
{
    public const int MaximumSizeInBytes = 5 * 1024 * 1024;
    public const int UrlMaxLength = 500;

    private const string UnsupportedImageMessage = "The image must be a PNG, JPG or WebP file.";
    private const string ImageTooLargeMessage = "The image must be 5 MB or smaller.";
    private const string ContentFieldName = "File";

    private CatalogImage(byte[] content, ImageFormat format)
    {
        Content = content;
        Format = format;
    }

    public byte[] Content { get; }
    public ImageFormat Format { get; }

    public static Result<CatalogImage> Create(byte[] content)
    {
        var format = ImageFormat.Detect(content);
        if (format is null)
        {
            return Result.Validation<CatalogImage>(UnsupportedImageMessage, ImageErrorCodes.Unsupported, ContentFieldName);
        }

        if (content.Length > MaximumSizeInBytes)
        {
            return Result.Validation<CatalogImage>(ImageTooLargeMessage, ImageErrorCodes.TooLarge, ContentFieldName);
        }

        return new CatalogImage(content, format);
    }
}
