using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;
using ClassManager.Storage.Images;

namespace ClassManager.Core.Domain.Images;

public static class CatalogImage
{
    public const int MaximumSizeInBytes = 5 * 1024 * 1024;

    private const string UnsupportedImageMessage = "The image must be a PNG, JPG or WebP file.";
    private const string ImageTooLargeMessage = "The image must be 5 MB or smaller.";
    private const string ContentFieldName = "File";

    public static Result<DocumentContent> Validate(byte[] content)
    {
        var format = ImageFormat.Detect(content);
        if (format is null)
        {
            return Result.Validation<DocumentContent>(UnsupportedImageMessage, ImageErrorCodes.Unsupported, ContentFieldName);
        }

        if (content.Length > MaximumSizeInBytes)
        {
            return Result.Validation<DocumentContent>(ImageTooLargeMessage, ImageErrorCodes.TooLarge, ContentFieldName);
        }

        return new DocumentContent(content, format.ContentType, format.FileExtension);
    }
}
