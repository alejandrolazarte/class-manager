using System.Globalization;
using ClassManager.Infrastructure;
using ClassManager.Infrastructure.Audit;
using ClassManager.Security.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Backoffice.Api.I.Tests.Infrastructure;

public sealed class BackofficeApiFactory(string databaseConnectionString, string auditLogConnectionString, long platformOrganizationId)
    : WebApplicationFactory<Program>
{
    public const string WebAppUrl = "https://admin.example.com";
    public const string BusinessWebAppUrl = "https://app.example.com";
    public const string TestSigningKey = "backoffice-tests-signing-key-with-more-than-32-bytes";

    private const string ConnectionStringsSection = "ConnectionStrings";
    private const string GoogleScheme = "google";
    private const string PlatformOrganizationSetting = "Backoffice:PlatformOrganizationId";
    private const string WebAppUrlSetting = "Backoffice:WebAppUrl";
    private const string BusinessWebAppUrlSetting = "WebApp:Url";
    private const string RateLimitSetting = "Authentication:RateLimit:PermitLimit";
    private const string TestPermitLimit = "10000";

    public static readonly DateTimeOffset Now = DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

    public FakeTimeProvider TimeProvider { get; } = new(Now);

    public HttpClient CreateHttpsClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
        BaseAddress = new Uri("https://localhost"),
        HandleCookies = true,
    });

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            $"{ConnectionStringsSection}:{InfrastructureServiceCollectionExtensions.DatabaseConnectionStringName}",
            databaseConnectionString);
        builder.UseSetting($"{ConnectionStringsSection}:{AuditLogOptions.ConnectionStringName}", auditLogConnectionString);
        builder.UseSetting($"{JwtOptions.SectionName}:{nameof(JwtOptions.SigningKey)}", TestSigningKey);
        builder.UseSetting(PlatformOrganizationSetting, platformOrganizationId.ToString(CultureInfo.InvariantCulture));
        builder.UseSetting(WebAppUrlSetting, WebAppUrl);
        builder.UseSetting(BusinessWebAppUrlSetting, BusinessWebAppUrl);
        builder.UseSetting(RateLimitSetting, TestPermitLimit);

        builder.ConfigureTestServices(services =>
        {
            services.Replace(ServiceDescriptor.Singleton<TimeProvider>(TimeProvider));
            services.Configure<AuthenticationOptions>(options => options.SchemeMap[GoogleScheme].HandlerType = typeof(FakeGoogleHandler));
        });
    }
}
