using ClassManager.Security.Tokens;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_key_is_too_short;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_startup_fails(ApiFixture fixture)
{
    private const string ShortSigningKey = "too-short";

    [Fact]
    public void Then_startup_fails_Run()
    {
        using var misconfiguredFactory = fixture.ApiFactory.WithWebHostBuilder(builder =>
            builder.UseSetting($"{JwtOptions.SectionName}:{nameof(JwtOptions.SigningKey)}", ShortSigningKey));

        Should.Throw<OptionsValidationException>(() => misconfiguredFactory.CreateClient());
    }
}
