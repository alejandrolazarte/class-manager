using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.Images;
using ClassManager.Storage.Files;
using Microsoft.Extensions.Logging;

namespace ClassManager.Infrastructure.Storage;

internal sealed partial class CatalogImageService(
    IFileStorage fileStorage,
    ITenantContext tenantContext,
    ILogger<CatalogImageService> logger)
    : ICatalogImageService
{
    private const string ProductsFolder = "products";
    private const string ClassPacksFolder = "class-packs";
    private const string GuidWithoutHyphensFormat = "N";

    public Task<Uri> SaveAsync(CatalogImageOwner owner, Guid ownerId, CatalogImage image, CancellationToken cancellationToken)
    {
        var fileName = Guid.CreateVersion7().ToString(GuidWithoutHyphensFormat) + image.Format.FileExtension;
        var path = FilePath.Combine(tenantContext.TenantId.ToString(), FolderOf(owner), ownerId.ToString(), fileName);

        return fileStorage.SaveAsync(new FileToStore(path, image.Content, image.Format.ContentType), cancellationToken);
    }

    public async Task DeleteAsync(string? imageUrl, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var parsedImageUrl))
        {
            return;
        }

        try
        {
            await fileStorage.DeleteAsync(parsedImageUrl, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            LogImageNotDeleted(logger, exception, parsedImageUrl);
        }
    }

    private static string FolderOf(CatalogImageOwner owner) => owner switch
    {
        CatalogImageOwner.Product => ProductsFolder,
        CatalogImageOwner.ClassPack => ClassPacksFolder,
        _ => throw new ArgumentOutOfRangeException(nameof(owner), owner, null),
    };

    [LoggerMessage(Level = LogLevel.Warning, Message = "The catalog image {ImageUrl} could not be deleted and stays orphaned in storage.")]
    private static partial void LogImageNotDeleted(ILogger logger, Exception exception, Uri imageUrl);
}
