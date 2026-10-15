using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_refreshing_from_the_web;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_cookie_refresh_token_is_rotated(ApiFixture fixture)
{
    [Fact]
    public async Task Then_cookie_refresh_token_is_rotated_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        using var signInResponse = await client.SignUpAndSignInFromWebAsync(fixture.ApiFactory.EmailTransport);
        var signInCookieValue = signInResponse.RefreshTokenCookieOf().Value.Value!;

        using var refreshResponse = await client.PostFromWebAsync(
            AuthenticationRequests.RefreshRoute, new RefreshSessionCommand(null), signInCookieValue);

        refreshResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        refreshResponse.RefreshTokenCookieOf().Value.Value.ShouldNotBe(signInCookieValue);
    }
}
