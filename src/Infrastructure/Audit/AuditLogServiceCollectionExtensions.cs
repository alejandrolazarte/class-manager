using Azure.Data.Tables;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ClassManager.Infrastructure.Audit;

internal static class AuditLogServiceCollectionExtensions
{
    public static IServiceCollection AddAuditLog(this IServiceCollection services)
    {
        services.AddOptions<AuditLogOptions>().BindConfiguration(AuditLogOptions.SectionName);
        services.AddSingleton<AuditLogChannel<CommandAuditEntry>>();
        services.AddSingleton<AuditLogChannel<SecurityEvent>>();
        services.AddSingleton<ICommandAuditService, QueuedCommandAuditService>();
        services.AddSingleton<ISecurityEventService, QueuedSecurityEventService>();
        services.AddScoped<UseCaseAuditRecorder>();
        services.AddSingleton<IAuditLogStore<CommandAuditEntry>>(serviceProvider =>
        {
            var connectionString = ConnectionStringOf(serviceProvider);
            if (connectionString is not null)
            {
                return new TableCommandAuditStore(new TableServiceClient(connectionString));
            }

            var tableServiceUri = OptionsOf(serviceProvider).TableServiceUri;
            return tableServiceUri is null
                ? new UnconfiguredAuditLogStore<CommandAuditEntry>()
                : new TableCommandAuditStore(new TableServiceClient(tableServiceUri, new DefaultAzureCredential()));
        });
        services.AddSingleton<IAuditLogStore<SecurityEvent>>(serviceProvider =>
        {
            var connectionString = ConnectionStringOf(serviceProvider);
            if (connectionString is not null)
            {
                return new AppendBlobSecurityEventStore(
                    new BlobServiceClient(connectionString).GetBlobContainerClient(AppendBlobSecurityEventStore.ContainerName),
                    createsContainer: true);
            }

            var blobServiceUri = OptionsOf(serviceProvider).BlobServiceUri;
            return blobServiceUri is null
                ? new UnconfiguredAuditLogStore<SecurityEvent>()
                : new AppendBlobSecurityEventStore(
                    new BlobServiceClient(blobServiceUri, new DefaultAzureCredential()).GetBlobContainerClient(AppendBlobSecurityEventStore.ContainerName),
                    createsContainer: false);
        });

        return services;
    }

    private static string? ConnectionStringOf(IServiceProvider serviceProvider)
    {
        var connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString(AuditLogOptions.ConnectionStringName);
        return string.IsNullOrWhiteSpace(connectionString) ? null : connectionString;
    }

    private static AuditLogOptions OptionsOf(IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<IOptions<AuditLogOptions>>().Value;
}
