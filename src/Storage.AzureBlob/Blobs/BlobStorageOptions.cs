namespace ClassManager.Storage.AzureBlob.Blobs;

public sealed class BlobStorageOptions
{
    public const string SectionName = "FileStorage";
    public const string ConnectionStringName = "FileStorage";
    public const string DefaultPublicContainerName = "public-files";
    public const string DefaultPrivateContainerName = "private-files";
    public const string DefaultCacheControl = "public, max-age=31536000, immutable";

    public string PublicContainerName { get; set; } = DefaultPublicContainerName;

    public string PrivateContainerName { get; set; } = DefaultPrivateContainerName;

    public string CacheControl { get; set; } = DefaultCacheControl;
}
