using ClassManager.Security.Tokens;
using Microsoft.Extensions.Options;

namespace ClassManager.Backoffice.Api.I.Tests.Authentication.When_an_app_token_calls_the_backoffice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_refused(BackofficeApiFixture fixture)
{
    private const string AppIssuer = "class-manager-api";
    private const string AppAudience = "class-manager-app";
    private const string AppSigningKey = "the-app-signing-key-with-more-than-32-bytes!";
    private const string TeamKind = "team";

    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        var appTokenFactory = new JwtAccessTokenFactory(
            Options.Create(new JwtOptions { Issuer = AppIssuer, Audience = AppAudience, SigningKey = AppSigningKey }),
            fixture.ApiFactory.TimeProvider);
        var appToken = appTokenFactory.Create(new TokenSubject(Guid.CreateVersion7(), "owner@example.com", TestIds.Next(), "BranchOwner", TeamKind)).Token;
        using var client = fixture.ApiFactory.CreateHttpsClient();

        using var response = await client.GetCurrentOperatorAsync(appToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
