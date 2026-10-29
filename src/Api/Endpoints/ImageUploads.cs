using ClassManager.Core.Domain.Images;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Api.Endpoints;

internal static class ImageUploads
{
    private const int MultipartOverheadInBytes = 64 * 1024;
    private const int UploadLimitInBytes = CatalogImage.MaximumSizeInBytes + MultipartOverheadInBytes;
    private const int PhotoUploadLimitInBytes = UserPhoto.MaximumUploadSizeInBytes + MultipartOverheadInBytes;

    public static RouteHandlerBuilder AcceptsImageUpload(this RouteHandlerBuilder builder) =>
        builder.DisableAntiforgery().WithFormOptions(multipartBodyLengthLimit: UploadLimitInBytes);

    public static RouteHandlerBuilder AcceptsPhotoUpload(this RouteHandlerBuilder builder) =>
        builder.DisableAntiforgery().WithFormOptions(multipartBodyLengthLimit: PhotoUploadLimitInBytes);

    public static async Task<byte[]> ReadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        using var content = new MemoryStream();
        await file.CopyToAsync(content, cancellationToken);
        return content.ToArray();
    }
}
