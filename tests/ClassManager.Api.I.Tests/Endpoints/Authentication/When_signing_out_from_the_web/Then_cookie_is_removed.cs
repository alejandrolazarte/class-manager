using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_out_from_the_web;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_cookie_is_removed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_cookie_is_removed_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        using var signInResponse = await client.SignUpAndSignInFromWebAsync(fixture.ApiFactory.EmailTransport);

        using var signOutResponse = await client.PostFromWebAsync(
            AuthenticationRequests.SignOutRoute, new SignOutCommand(null), signInResponse.RefreshTokenCookieOf().Value.Value);

        signOutResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        signOutResponse.RefreshTokenCookieOf().Expires.ShouldNotBeNull().ShouldBeLessThan(DateTimeOffset.UtcNow);
    }
}
