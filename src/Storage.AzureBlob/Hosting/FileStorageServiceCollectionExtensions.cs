using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ClassManager.Storage.AzureBlob.Blobs;
using ClassManager.Storage.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ClassManager.Storage.AzureBlob.Hosting;

public static class FileStorageServiceCollectionExtensions
{
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
                return new UnconfiguredFileStorage();
            }

            var serviceClient = new BlobServiceClient(connectionString);
            return new AzureBlobFileStorage(
                new BlobContainer(serviceClient.GetBlobContainerClient(options.PublicContainerName), PublicAccessType.Blob),
                new BlobContainer(serviceClient.GetBlobContainerClient(options.PrivateContainerName), PublicAccessType.None),
                options.CacheControl);
        });

        return services;
    }
}
