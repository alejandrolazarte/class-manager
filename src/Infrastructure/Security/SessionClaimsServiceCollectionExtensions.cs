using ClassManager.Core.Abstractions.Security;
using ClassManager.Tenancy.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Infrastructure.Security;

public static class SessionClaimsServiceCollectionExtensions
{
    public static IServiceCollection AddSessionClaims(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, ClaimsCurrentUser>();
        services.AddClaimsTenancy();

        return services;
    }
}
