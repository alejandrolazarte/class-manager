using ClassManager.Security.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_authentication_requests_exceed_the_rate_limit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_429(ApiFixture fixture)
{
    private const int PermitLimit = 2;

    [Fact]
    public async Task Then_returns_429_Run()
    {
        using var rateLimitedFactory = fixture.ApiFactory.WithWebHostBuilder(builder => builder.UseSetting(
            $"{AuthenticationRateLimitOptions.SectionName}:{nameof(AuthenticationRateLimitOptions.PermitLimit)}",
            PermitLimit.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        using var client = rateLimitedFactory.CreateClient();
        for (var attempt = 0; attempt < PermitLimit; attempt++)
        {
            using var permittedResponse = await client.PostSignInAsync(AuthenticationRequests.UniqueEmail(), AuthenticationRequests.WrongPassword);
            permittedResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        using var response = await client.PostSignInAsync(AuthenticationRequests.UniqueEmail(), AuthenticationRequests.WrongPassword);

        response.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
    }
}
