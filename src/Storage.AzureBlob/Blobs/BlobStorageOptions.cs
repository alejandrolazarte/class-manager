namespace ClassManager.Storage.AzureBlob.Blobs;

public sealed class BlobStorageOptions
{
    public const string SectionName = "FileStorage";
    public const string ConnectionStringName = "FileStorage";
    public const string DefaultContainerName = "public-files";
    public const string DefaultCacheControl = "public, max-age=31536000, immutable";

    public string ContainerName { get; set; } = DefaultContainerName;

    public string CacheControl { get; set; } = DefaultCacheControl;
}
