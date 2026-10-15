using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_refreshing_with_a_cookie_without_the_web_header;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_session_is_not_refreshed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_session_is_not_refreshed_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        using var signInResponse = await client.SignUpAndSignInFromWebAsync(fixture.ApiFactory.EmailTransport);

        using var refreshResponse = await client.PostFromWebAsync(
            AuthenticationRequests.RefreshRoute,
            new RefreshSessionCommand(null),
            signInResponse.RefreshTokenCookieOf().Value.Value,
            sendsCookieHeader: false);

        refreshResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
