using ClassManager.Infrastructure;
using ClassManager.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Backoffice.Infrastructure;

public static class BackofficeInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddBackofficeInfrastructure(this IServiceCollection services) =>
        services
            .AddInfrastructure()
            .AddSessionClaims()
            .AddUseCaseBehaviors();
}
