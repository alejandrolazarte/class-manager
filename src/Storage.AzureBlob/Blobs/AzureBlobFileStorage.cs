using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ClassManager.Storage.Files;

namespace ClassManager.Storage.AzureBlob.Blobs;

internal sealed class AzureBlobFileStorage(BlobContainerClient containerClient, string cacheControl) : IFileStorage, IDisposable
{
    private readonly SemaphoreSlim _containerCreation = new(1, 1);
    private bool _containerExists;

    public async Task<Uri> SaveAsync(FileToStore file, CancellationToken cancellationToken)
    {
        await EnsureContainerExistsAsync(cancellationToken);
        var blobClient = containerClient.GetBlobClient(file.Path);
        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType, CacheControl = cacheControl },
        };
        await blobClient.UploadAsync(BinaryData.FromBytes(file.Content), uploadOptions, cancellationToken);

        return blobClient.Uri;
    }

    public async Task<bool> DeleteAsync(Uri fileUrl, CancellationToken cancellationToken)
    {
        var path = PathOf(fileUrl);
        if (path is null)
        {
            return false;
        }

        var response = await containerClient.GetBlobClient(path).DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return response.Value;
    }

    public void Dispose() => _containerCreation.Dispose();

    private string? PathOf(Uri fileUrl)
    {
        var containerPrefix = containerClient.Uri.AbsoluteUri.TrimEnd(FilePath.Separator) + FilePath.Separator;
        var fileAddress = fileUrl.AbsoluteUri;
        if (!fileAddress.StartsWith(containerPrefix, StringComparison.Ordinal) || fileAddress.Length == containerPrefix.Length)
        {
            return null;
        }

        return Uri.UnescapeDataString(fileAddress[containerPrefix.Length..]);
    }

    private async Task EnsureContainerExistsAsync(CancellationToken cancellationToken)
    {
        if (_containerExists)
        {
            return;
        }

        await _containerCreation.WaitAsync(cancellationToken);
        try
        {
            if (!_containerExists)
            {
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);
                _containerExists = true;
            }
        }
        finally
        {
            _containerCreation.Release();
        }
    }
}
