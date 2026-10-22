using Azure.Data.Tables;
using Azure.Identity;
using Azure.Storage.Blobs;
using ClassManager.Backoffice.Core.Logs;
using ClassManager.Infrastructure.Audit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ClassManager.Backoffice.Infrastructure.Logs;

internal static class AuditLogReaderServiceCollectionExtensions
{
    public static IServiceCollection AddAuditLogReader(this IServiceCollection services)
    {
        services.AddScoped<IAuditLogDirectory, AuditLogDirectory>();
        services.AddSingleton<IAuditLogReader>(serviceProvider =>
        {
            var connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString(AuditLogOptions.ConnectionStringName);
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return new AzureAuditLogReader(
                    new TableServiceClient(connectionString),
                    new BlobServiceClient(connectionString).GetBlobContainerClient(AppendBlobSecurityEventStore.ContainerName));
            }

            var options = serviceProvider.GetRequiredService<IOptions<AuditLogOptions>>().Value;
            if (options.TableServiceUri is null || options.BlobServiceUri is null)
            {
                return new UnconfiguredAuditLogReader();
            }

            var credential = new DefaultAzureCredential();
            return new AzureAuditLogReader(
                new TableServiceClient(options.TableServiceUri, credential),
                new BlobServiceClient(options.BlobServiceUri, credential).GetBlobContainerClient(AppendBlobSecurityEventStore.ContainerName));
        });

        return services;
    }
}
