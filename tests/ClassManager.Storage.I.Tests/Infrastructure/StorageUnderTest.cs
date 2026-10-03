using Azure.Storage.Blobs;

namespace ClassManager.Storage.I.Tests.Infrastructure;

public sealed record StorageUnderTest(IFileStorage Files, BlobServiceClient ServiceClient, string PrivateContainerName)
{
    public Uri PrivateUrlOf(string path) => ServiceClient.GetBlobContainerClient(PrivateContainerName).GetBlobClient(path).Uri;

    public async Task<bool> PrivateFileExistsAsync(string path) =>
        (await ServiceClient.GetBlobContainerClient(PrivateContainerName).GetBlobClient(path).ExistsAsync()).Value;
}
