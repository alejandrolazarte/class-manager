using ClassManager.Core.Common;
using ClassManager.Core.Domain.Images;
using ClassManager.Storage.Images;

namespace ClassManager.Core.Domain.Users;

public static class UserPhotoUpload
{
    private const string UnsupportedImageMessage = "The photo must be a PNG, JPG or WebP file.";
    private const string PhotoTooLargeMessage = "The photo must be 10 MB or smaller.";
    private const string ContentFieldName = "File";

    public static Result Validate(byte[] content)
    {
        if (content.Length > UserPhoto.MaximumUploadSizeInBytes)
        {
            return Result.Validation(PhotoTooLargeMessage, UserPhotoErrorCodes.TooLarge, ContentFieldName);
        }

        return ImageFormat.Detect(content) is null
            ? Unsupported()
            : Result.Success();
    }

    public static Result Unsupported() =>
        Result.Validation(UnsupportedImageMessage, ImageErrorCodes.Unsupported, ContentFieldName);
}
