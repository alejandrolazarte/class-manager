using ClassManager.Core.Abstractions.Storage;
using ClassManager.Storage.Files;
using Microsoft.Extensions.Logging;

namespace ClassManager.Infrastructure.Storage;

internal sealed partial class DocumentStorageService(
    IFileStorage fileStorage,
    ITenantContext tenantContext,
    TimeProvider timeProvider,
    ILogger<DocumentStorageService> logger)
    : IDocumentStorageService
{
    private const string ProductsFolder = "products";
    private const string ClassPacksFolder = "class-packs";
    private const string GuidWithoutHyphensFormat = "N";
    private const string PrivateDocumentUrlMessage = "Private documents have no public URL.";

    public async Task<Document> StoreAsync(
        DocumentOwner owner,
        Guid ownerId,
        DocumentContent content,
        DocumentVisibility visibility,
        CancellationToken cancellationToken)
    {
        var documentId = Guid.CreateVersion7();
        var fileName = documentId.ToString(GuidWithoutHyphensFormat) + content.FileExtension;
        var path = FilePath.Combine(tenantContext.TenantId.ToString(), FolderOf(owner), ownerId.ToString(), fileName);
        await fileStorage.SaveAsync(new FileToStore(path, content.Content, content.ContentType, ToFileVisibility(visibility)), cancellationToken);

        return Document.Create(documentId, path, content, visibility, timeProvider.GetUtcNow());
    }

    public async Task DeleteFileAsync(Document document, CancellationToken cancellationToken)
    {
        try
        {
            await fileStorage.DeleteAsync(document.Path, ToFileVisibility(document.Visibility), cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            LogFileNotDeleted(logger, exception, document.Path);
        }
    }

    public Uri PublicUrlOf(Document document) =>
        document.Visibility == DocumentVisibility.Public
            ? fileStorage.PublicUrlOf(document.Path)
            : throw new InvalidOperationException(PrivateDocumentUrlMessage);

    private static FileVisibility ToFileVisibility(DocumentVisibility visibility) =>
        visibility == DocumentVisibility.Public ? FileVisibility.Public : FileVisibility.Private;

    private static string FolderOf(DocumentOwner owner) => owner switch
    {
        DocumentOwner.Product => ProductsFolder,
        DocumentOwner.ClassPack => ClassPacksFolder,
        _ => throw new ArgumentOutOfRangeException(nameof(owner), owner, null),
    };

    [LoggerMessage(Level = LogLevel.Warning, Message = "The file {Path} could not be deleted and stays orphaned in storage.")]
    private static partial void LogFileNotDeleted(ILogger logger, Exception exception, string path);
}
