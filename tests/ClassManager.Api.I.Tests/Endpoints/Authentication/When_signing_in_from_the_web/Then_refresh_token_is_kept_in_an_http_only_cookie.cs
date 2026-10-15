using ClassManager.Api.Authentication;
using Microsoft.Net.Http.Headers;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_in_from_the_web;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_refresh_token_is_kept_in_an_http_only_cookie(ApiFixture fixture)
{
    [Fact]
    public async Task Then_refresh_token_is_kept_in_an_http_only_cookie_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();

        using var response = await client.SignUpAndSignInFromWebAsync(fixture.ApiFactory.EmailTransport);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var cookie = response.RefreshTokenCookieOf();
        cookie.Value.Value.ShouldNotBeNullOrEmpty();
        cookie.HttpOnly.ShouldBeTrue();
        cookie.Secure.ShouldBeTrue();
        cookie.SameSite.ShouldBe(SameSiteMode.Strict);
        cookie.Path.Value.ShouldBe(RefreshTokenCookie.Path);
        cookie.Expires.ShouldNotBeNull();
    }
}
