using ClassManager.Core.Domain.Images;

namespace ClassManager.Api.Endpoints;

internal static class ImageUploads
{
    private const int MultipartOverheadInBytes = 64 * 1024;
    private const int UploadLimitInBytes = CatalogImage.MaximumSizeInBytes + MultipartOverheadInBytes;

    public static RouteHandlerBuilder AcceptsImageUpload(this RouteHandlerBuilder builder) =>
        builder.DisableAntiforgery().WithFormOptions(multipartBodyLengthLimit: UploadLimitInBytes);

    public static async Task<byte[]> ReadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        using var content = new MemoryStream();
        await file.CopyToAsync(content, cancellationToken);
        return content.ToArray();
    }
}
