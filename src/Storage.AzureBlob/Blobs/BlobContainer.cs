using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ClassManager.Storage.AzureBlob.Blobs;

internal sealed class BlobContainer(BlobContainerClient client, PublicAccessType accessType) : IDisposable
{
    private readonly SemaphoreSlim _creation = new(1, 1);
    private bool _exists;

    public BlobContainerClient Client => client;

    public async Task EnsureExistsAsync(CancellationToken cancellationToken)
    {
        if (_exists)
        {
            return;
        }

        await _creation.WaitAsync(cancellationToken);
        try
        {
            if (!_exists)
            {
                await client.CreateIfNotExistsAsync(accessType, cancellationToken: cancellationToken);
                _exists = true;
            }
        }
        finally
        {
            _creation.Release();
        }
    }

    public void Dispose() => _creation.Dispose();
}
