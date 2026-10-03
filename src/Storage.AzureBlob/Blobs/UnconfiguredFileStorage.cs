using ClassManager.Storage.Files;

namespace ClassManager.Storage.AzureBlob.Blobs;

internal sealed class UnconfiguredFileStorage : IFileStorage
{
    private const string MissingConnectionStringMessage = "Connection string 'FileStorage' is not configured.";

    public Task SaveAsync(FileToStore file, CancellationToken cancellationToken) => throw NotConfigured();

    public Task<bool> DeleteAsync(string path, FileVisibility visibility, CancellationToken cancellationToken) => throw NotConfigured();

    public Uri PublicUrlOf(string path) => throw NotConfigured();

    private static InvalidOperationException NotConfigured() => new(MissingConnectionStringMessage);
}
