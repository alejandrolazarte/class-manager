using ClassManager.Infrastructure.Email;
using ClassManager.Infrastructure.WebPush;
using ClassManager.Security.Hosting;
using ClassManager.Security.Tokens;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed class BusinessApiFactory(string databaseConnectionString, TimeProvider timeProvider) : WebApplicationFactory<Program>
{
    public const string TestSigningKey = "integration-tests-signing-key-with-more-than-32-bytes";
    public const int TestAuthenticationPermitLimit = 10_000;

    private const string ConnectionStringsSection = "ConnectionStrings";

    public static readonly DateTimeOffset Now = new(2026, 9, 24, 12, 0, 0, TimeSpan.Zero);

    private static readonly VapidKeys TestVapidKeys = VapidKeys.Generate();

    public static string VapidPublicKey => TestVapidKeys.PublicKey;

    public RecordingEmailTransport EmailTransport { get; } = new();

    internal RecordingWebPushSender PushSender { get; } = new();

    public BusinessApiFactory(string databaseConnectionString)
        : this(databaseConnectionString, new FakeTimeProvider(Now))
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            $"{ConnectionStringsSection}:{InfrastructureServiceCollectionExtensions.DatabaseConnectionStringName}",
            databaseConnectionString);
        builder.UseSetting($"{JwtOptions.SectionName}:{nameof(JwtOptions.SigningKey)}", TestSigningKey);
        builder.UseSetting($"{VapidOptions.SectionName}:{nameof(VapidOptions.Subject)}", "mailto:tests@example.com");
        builder.UseSetting($"{VapidOptions.SectionName}:{nameof(VapidOptions.PublicKey)}", TestVapidKeys.PublicKey);
        builder.UseSetting($"{VapidOptions.SectionName}:{nameof(VapidOptions.PrivateKey)}", TestVapidKeys.PrivateKey);
        builder.UseSetting(
            $"{AuthenticationRateLimitOptions.SectionName}:{nameof(AuthenticationRateLimitOptions.PermitLimit)}",
            TestAuthenticationPermitLimit.ToString(System.Globalization.CultureInfo.InvariantCulture));
        builder.ConfigureTestServices(services =>
        {
            services.Replace(ServiceDescriptor.Singleton(timeProvider));
            services.Replace(ServiceDescriptor.Singleton<IEmailTransport>(EmailTransport));
            services.Replace(ServiceDescriptor.Singleton<IWebPushSender>(PushSender));
        });
    }
}
