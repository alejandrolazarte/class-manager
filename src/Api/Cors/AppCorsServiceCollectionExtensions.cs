using Microsoft.Net.Http.Headers;

namespace ClassManager.Api.Cors;

public static class AppCorsServiceCollectionExtensions
{
    public const string AllowedOriginsSection = "Cors:AllowedOrigins";
    public const string AppPolicyName = "ClassManagerApp";

    public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection(AllowedOriginsSection).Get<string[]>() ?? [];

        services.AddCors(options => options.AddPolicy(AppPolicyName, policy => policy
            .WithOrigins(allowedOrigins)
            .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization)
            .WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete)));

        return services;
    }
}
