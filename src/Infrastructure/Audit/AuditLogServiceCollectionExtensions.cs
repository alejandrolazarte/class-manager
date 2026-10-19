using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ClassManager.Infrastructure.Audit;

internal static class AuditLogServiceCollectionExtensions
{
    public static IServiceCollection AddAuditLog(this IServiceCollection services)
    {
        services.AddOptions<AuditLogOptions>().BindConfiguration(AuditLogOptions.SectionName);
        services.AddSingleton<CommandAuditChannel>();
        services.AddSingleton<ICommandAuditService, QueuedCommandAuditService>();
        services.AddSingleton<ICommandAuditStore>(serviceProvider =>
        {
            var connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString(AuditLogOptions.ConnectionStringName);
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return new TableCommandAuditStore(new TableServiceClient(connectionString));
            }

            var tableServiceUri = serviceProvider.GetRequiredService<IOptions<AuditLogOptions>>().Value.TableServiceUri;
            return tableServiceUri is null
                ? new UnconfiguredCommandAuditStore()
                : new TableCommandAuditStore(new TableServiceClient(tableServiceUri, new DefaultAzureCredential()));
        });

        return services;
    }
}
