using Azure.Storage.Blobs;
using ClassManager.Storage.AzureBlob.Blobs;
using ClassManager.Storage.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ClassManager.Storage.AzureBlob.Hosting;

public static class FileStorageServiceCollectionExtensions
{
    private const string MissingConnectionStringMessage = "Connection string 'FileStorage' is not configured.";

    public static IServiceCollection AddAzureBlobFileStorage(this IServiceCollection services)
    {
        services.AddOptions<BlobStorageOptions>().BindConfiguration(BlobStorageOptions.SectionName);
        services.AddSingleton<IFileStorage>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var options = serviceProvider.GetRequiredService<IOptions<BlobStorageOptions>>().Value;
            var connectionString = configuration.GetConnectionString(BlobStorageOptions.ConnectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(MissingConnectionStringMessage);
            }

            var containerClient = new BlobServiceClient(connectionString).GetBlobContainerClient(options.ContainerName);
            return new AzureBlobFileStorage(containerClient, options.CacheControl);
        });

        return services;
    }
}
