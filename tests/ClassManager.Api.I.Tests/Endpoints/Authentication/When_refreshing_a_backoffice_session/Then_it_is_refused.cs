using System.Globalization;
using System.Text;
using System.Text.Json;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Security.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_refreshing_a_backoffice_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_refused(ApiFixture fixture)
{
    private const string BackofficeKind = "backoffice";
    private const string TenantClaimType = "tenant_id";
    private const char TokenPartSeparator = '.';

    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        var appTokens = await client.SignUpAsync(fixture.ApiFactory.EmailTransport, command);
        var claims = ClaimsOf(appTokens.AccessToken);
        using var scope = fixture.ApiFactory.Services.CreateScope();
        var backofficeTokens = await scope.ServiceProvider.GetRequiredService<ITokenIssuer>().IssueAsync(
            new TokenSubject(
                Guid.Parse(claims.GetProperty(SecurityClaimTypes.Subject).GetString()!),
                command.Email!,
                long.Parse(claims.GetProperty(TenantClaimType).GetString()!, CultureInfo.InvariantCulture),
                nameof(Core.Domain.Businesses.BusinessRole.PlatformOperator),
                BackofficeKind),
            CancellationToken.None);

        using var response = await client.PostRefreshAsync(backofficeTokens.RefreshToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(AuthenticationErrorCodes.InvalidRefreshToken);
    }

    private static JsonElement ClaimsOf(string accessToken)
    {
        var payload = accessToken.Split(TokenPartSeparator)[1].Replace('-', '+').Replace('_', '/');
        var padded = payload.PadRight(payload.Length + ((4 - (payload.Length % 4)) % 4), '=');
        return JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(padded))).RootElement;
    }
}
