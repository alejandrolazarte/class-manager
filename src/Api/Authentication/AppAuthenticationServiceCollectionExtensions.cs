using ClassManager.Core.Abstractions.Security;
using ClassManager.Security.Hosting;
using ClassManager.Tenancy.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;

namespace ClassManager.Api.Authentication;

internal static class AppAuthenticationServiceCollectionExtensions
{
    private const int ForwardedProxyHopCount = 1;

    public static IServiceCollection AddAppAuthentication(this IServiceCollection services)
    {
        services.AddSecurityAuthentication();

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            .AddPolicy(
                AuthorizationPolicies.Student,
                policy => policy.RequireAuthenticatedUser().AddRequirements(new StudentRequirement()))
            .AddPolicy(AuthorizationPolicies.AnyAccount, policy => policy.RequireAuthenticatedUser());
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, PlanAwareAuthorizationResultHandler>();
        services.AddScoped<IAuthorizationHandler, StudentAuthorizationHandler>();
        services.AddScoped<ICurrentUser, ClaimsCurrentUser>();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = ForwardedProxyHopCount;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddClaimsTenancy();

        return services;
    }
}
