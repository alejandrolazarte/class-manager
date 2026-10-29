using System.Globalization;
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
    private const string ClassGroupsFolder = "class-groups";
    private const string AnnouncementsFolder = "announcements";
    private const string UsersFolder = "users";
    private const string RandomFileNameFormat = "N";
    private const string PrivateDocumentUrlMessage = "Private documents have no public URL.";
    private const string PublicDocumentSignedUrlMessage = "Public documents need no signed URL.";

    private static readonly TimeSpan SignedUrlWindow = TimeSpan.FromHours(1);
    private static readonly TimeProvider StorageServiceClock = TimeProvider.System;

    public async Task<Document> StoreAsync(
        DocumentOwner owner,
        long ownerId,
        DocumentContent content,
        DocumentVisibility visibility,
        CancellationToken cancellationToken)
    {
        var fileName = Guid.NewGuid().ToString(RandomFileNameFormat, CultureInfo.InvariantCulture) + content.FileExtension;
        var path = FilePath.Combine(
            tenantContext.TenantId.ToString(CultureInfo.InvariantCulture),
            FolderOf(owner),
            ownerId.ToString(CultureInfo.InvariantCulture),
            fileName);
        await fileStorage.SaveAsync(new FileToStore(path, content.Content, content.ContentType, ToFileVisibility(visibility)), cancellationToken);

        return Document.Create(path, content, visibility, timeProvider.GetUtcNow());
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

    public Uri PrivateUrlOf(Document document) =>
        document.Visibility == DocumentVisibility.Private
            ? fileStorage.PrivateReadUrlOf(document.Path, SignedUrlExpiry(StorageServiceClock.GetUtcNow()))
            : throw new InvalidOperationException(PublicDocumentSignedUrlMessage);

    private static DateTimeOffset SignedUrlExpiry(DateTimeOffset now)
    {
        var windowStart = new DateTimeOffset(now.UtcTicks - (now.UtcTicks % SignedUrlWindow.Ticks), TimeSpan.Zero);
        return windowStart + SignedUrlWindow + SignedUrlWindow;
    }

    private static FileVisibility ToFileVisibility(DocumentVisibility visibility) =>
        visibility == DocumentVisibility.Public ? FileVisibility.Public : FileVisibility.Private;

    private static string FolderOf(DocumentOwner owner) => owner switch
    {
        DocumentOwner.Product => ProductsFolder,
        DocumentOwner.ClassPack => ClassPacksFolder,
        DocumentOwner.ClassGroup => ClassGroupsFolder,
        DocumentOwner.Announcement => AnnouncementsFolder,
        DocumentOwner.User => UsersFolder,
        _ => throw new ArgumentOutOfRangeException(nameof(owner), owner, null),
    };

    [LoggerMessage(Level = LogLevel.Warning, Message = "The file {Path} could not be deleted and stays orphaned in storage.")]
    private static partial void LogFileNotDeleted(ILogger logger, Exception exception, string path);
}
