using Azure.Storage.Blobs.Models;
using ClassManager.Storage.Files;

namespace ClassManager.Storage.AzureBlob.Blobs;

internal sealed class AzureBlobFileStorage(BlobContainer publicContainer, BlobContainer privateContainer, string cacheControl)
    : IFileStorage, IDisposable
{
    private const string PrivateCacheControl = "private, no-store";

    public async Task SaveAsync(FileToStore file, CancellationToken cancellationToken)
    {
        var container = ContainerFor(file.Visibility);
        await container.EnsureExistsAsync(cancellationToken);
        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType,
                CacheControl = file.Visibility == FileVisibility.Public ? cacheControl : PrivateCacheControl,
            },
        };
        await container.Client.GetBlobClient(file.Path).UploadAsync(BinaryData.FromBytes(file.Content), uploadOptions, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string path, FileVisibility visibility, CancellationToken cancellationToken)
    {
        var response = await ContainerFor(visibility).Client.GetBlobClient(path).DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return response.Value;
    }

    public Uri PublicUrlOf(string path) => publicContainer.Client.GetBlobClient(path).Uri;

    public void Dispose()
    {
        publicContainer.Dispose();
        privateContainer.Dispose();
    }

    private BlobContainer ContainerFor(FileVisibility visibility) =>
        visibility == FileVisibility.Public ? publicContainer : privateContainer;
}
