using ClassManager.Infrastructure;
using ClassManager.Security.Tokens;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClassManager.Backoffice.Api.I.Tests.Infrastructure;

public sealed class BackofficeApiFactory(string databaseConnectionString) : WebApplicationFactory<Program>
{
    private const string ConnectionStringsSection = "ConnectionStrings";
    private const string TestSigningKey = "backoffice-tests-signing-key-with-more-than-32-bytes";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            $"{ConnectionStringsSection}:{InfrastructureServiceCollectionExtensions.DatabaseConnectionStringName}",
            databaseConnectionString);
        builder.UseSetting($"{JwtOptions.SectionName}:{nameof(JwtOptions.SigningKey)}", TestSigningKey);
    }
}
