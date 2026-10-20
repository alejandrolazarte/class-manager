using ClassManager.Backoffice.Core.Operators;
using ClassManager.Backoffice.Infrastructure;
using ClassManager.Security.Hosting;
using ClassManager.Security.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Net.Http.Headers;

namespace ClassManager.Backoffice.Api.Authentication;

internal static class BackofficeAuthenticationServiceCollectionExtensions
{
    public const string CorsPolicyName = "LazttyBackoffice";

    private const string AllowedOriginsSection = "Cors:AllowedOrigins";
    private const int ForwardedProxyHopCount = 1;

    private static readonly TimeSpan ExternalSignInLifetime = TimeSpan.FromMinutes(5);
    private static readonly string[] GoogleScopes = ["openid", "email", "profile"];

    public static IServiceCollection AddBackofficeAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSecurityAuthentication();
        services.AddAuthentication()
            .AddCookie(BackofficeAuthenticationSchemes.External, options =>
            {
                options.Cookie.Name = BackofficeAuthenticationSchemes.ExternalCookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                options.ExpireTimeSpan = ExternalSignInLifetime;
            })
            .AddOpenIdConnect(BackofficeAuthenticationSchemes.Google, _ => { });
        services.AddOptions<OpenIdConnectOptions>(BackofficeAuthenticationSchemes.Google)
            .Configure<IOptions<BackofficeOptions>>(ConfigureGoogle);

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireClaim(SecurityClaimTypes.Kind, BackofficeSession.Kind)
                .Build());

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = ForwardedProxyHopCount;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        var allowedOrigins = configuration.GetSection(AllowedOriginsSection).Get<string[]>() ?? [];
        services.AddCors(options => options.AddPolicy(CorsPolicyName, policy => policy
            .WithOrigins(allowedOrigins)
            .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, BackofficeRefreshTokenCookie.RequestHeaderName)
            .WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete)
            .AllowCredentials()));

        return services;
    }

    private static void ConfigureGoogle(OpenIdConnectOptions options, IOptions<BackofficeOptions> backofficeOptions)
    {
        var google = backofficeOptions.Value.Google;
        options.Authority = google.Authority;
        options.ClientId = google.ClientId;
        options.ClientSecret = google.ClientSecret;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.CallbackPath = BackofficeAuthenticationSchemes.GoogleCallbackPath;
        options.SignInScheme = BackofficeAuthenticationSchemes.External;
        options.MapInboundClaims = false;
        options.SaveTokens = false;
        options.Scope.Clear();
        foreach (var scope in GoogleScopes)
        {
            options.Scope.Add(scope);
        }
    }
}
