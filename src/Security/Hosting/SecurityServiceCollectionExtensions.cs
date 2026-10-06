using System.Threading.RateLimiting;
using ClassManager.Security.Accounts;
using ClassManager.Security.Persistence;
using ClassManager.Security.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ClassManager.Security.Hosting;

public static class SecurityServiceCollectionExtensions
{
    public const string AuthenticationRateLimitPolicy = "authentication";

    private const string UnknownClientPartition = "unknown";
    private const string MissingDbContextConfigurationMessage = "SecurityOptions.ConfigureDbContext must be set.";

    private static readonly TimeSpan TokenClockSkew = TimeSpan.FromSeconds(30);

    /// <summary>
    /// User accounts (ASP.NET Core Identity), password reset tokens, token issuing with refresh token rotation and <see cref="SecurityDbContext"/>.
    /// The application must also register an <see cref="ITokenSubjectResolver"/>.
    /// </summary>
    public static IServiceCollection AddSecurityServices(this IServiceCollection services, Action<SecurityOptions> configure)
    {
        var securityOptions = new SecurityOptions();
        configure(securityOptions);
        var configureDbContext = securityOptions.ConfigureDbContext
            ?? throw new InvalidOperationException(MissingDbContextConfigurationMessage);

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .PostConfigure(jwtOptions => jwtOptions.TenantClaimType = securityOptions.TenantClaimType)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<SecurityDbContext>(configureDbContext);
        services
            .AddIdentityCore<ApplicationUser>(identityOptions => ConfigureIdentity(identityOptions, securityOptions))
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<SecurityDbContext>();

        services.AddSingleton<IAccessTokenFactory, JwtAccessTokenFactory>();
        services.AddScoped<ITokenIssuer, JwtTokenIssuer>();
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.Configure<PasswordResetOptions>(passwordResetOptions =>
            passwordResetOptions.TokenLifetime = securityOptions.PasswordResetTokenLifetime);
        services.AddScoped<IPasswordResetService, PasswordResetService>();
        services.Configure<EmailChangeOptions>(emailChangeOptions =>
            emailChangeOptions.TokenLifetime = securityOptions.EmailChangeTokenLifetime);
        services.AddScoped<IEmailChangeService, EmailChangeService>();

        return services;
    }

    /// <summary>
    /// JWT bearer validation for the tokens issued by <see cref="AddSecurityServices"/>, and the
    /// <see cref="AuthenticationRateLimitPolicy"/> rate limit for sign-in style endpoints.
    /// </summary>
    public static IServiceCollection AddSecurityAuthentication(this IServiceCollection services)
    {
        services.AddOptions<AuthenticationRateLimitOptions>()
            .BindConfiguration(AuthenticationRateLimitOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>, TimeProvider>(ConfigureJwtBearer);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(AuthenticationRateLimitPolicy, CreateAuthenticationRateLimitPartition);
        });

        return services;
    }

    private static void ConfigureIdentity(IdentityOptions options, SecurityOptions securityOptions)
    {
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = string.Empty;

        options.Password.RequiredLength = securityOptions.PasswordMinLength;
        options.Password.RequiredUniqueChars = 1;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = securityOptions.MaxFailedSignInAttempts;
        options.Lockout.DefaultLockoutTimeSpan = securityOptions.LockoutDuration;
    }

    private static void ConfigureJwtBearer(JwtBearerOptions bearerOptions, IOptions<JwtOptions> jwtOptions, TimeProvider timeProvider)
    {
        bearerOptions.MapInboundClaims = false;
        bearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtOptions.Value.Issuer,
            ValidAudience = jwtOptions.Value.Audience,
            IssuerSigningKey = jwtOptions.Value.CreateSigningKey(),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            NameClaimType = SecurityClaimTypes.Subject,
            RoleClaimType = SecurityClaimTypes.Role,
            ClockSkew = TokenClockSkew,
            LifetimeValidator = (notBefore, expires, _, parameters) =>
                IsWithinLifetime(notBefore, expires, parameters.ClockSkew, timeProvider.GetUtcNow().UtcDateTime),
        };
    }

    private static bool IsWithinLifetime(DateTime? notBefore, DateTime? expires, TimeSpan clockSkew, DateTime utcNow) =>
        expires is not null
        && utcNow < expires.Value.ToUniversalTime() + clockSkew
        && (notBefore is null || utcNow >= notBefore.Value.ToUniversalTime() - clockSkew);

    private static RateLimitPartition<string> CreateAuthenticationRateLimitPartition(HttpContext httpContext)
    {
        var rateLimit = httpContext.RequestServices.GetRequiredService<IOptions<AuthenticationRateLimitOptions>>().Value;
        var clientAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? UnknownClientPartition;

        return RateLimitPartition.GetFixedWindowLimiter(clientAddress, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = rateLimit.PermitLimit,
            Window = rateLimit.Window,
            QueueLimit = 0,
        });
    }
}
