using ClassManager.Tenancy.AspNetCore.Claims;
using ClassManager.Tenancy.AspNetCore.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Tenancy.AspNetCore.Hosting;

public static class TenancyServiceCollectionExtensions
{
    public static IServiceCollection AddClaimsTenancy(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ClaimsTenantContext>();
        services.AddScoped<ITenantContext>(serviceProvider => serviceProvider.GetRequiredService<ClaimsTenantContext>());
        services.AddScoped<ITenantScope>(serviceProvider => serviceProvider.GetRequiredService<ClaimsTenantContext>());

        return services;
    }

    public static IServiceCollection AddTenantStamping(this IServiceCollection services)
    {
        services.AddScoped<TenantStampingSaveChangesInterceptor>();

        return services;
    }
}
