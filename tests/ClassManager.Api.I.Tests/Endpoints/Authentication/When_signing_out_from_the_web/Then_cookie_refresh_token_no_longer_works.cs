using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_out_from_the_web;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_cookie_refresh_token_no_longer_works(ApiFixture fixture)
{
    [Fact]
    public async Task Then_cookie_refresh_token_no_longer_works_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        using var signInResponse = await client.SignUpAndSignInFromWebAsync(fixture.ApiFactory.EmailTransport);
        var cookieValue = signInResponse.RefreshTokenCookieOf().Value.Value;
        using var signOutResponse = await client.PostFromWebAsync(
            AuthenticationRequests.SignOutRoute, new SignOutCommand(null), cookieValue);

        using var refreshResponse = await client.PostFromWebAsync(
            AuthenticationRequests.RefreshRoute, new RefreshSessionCommand(null), cookieValue);

        refreshResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
