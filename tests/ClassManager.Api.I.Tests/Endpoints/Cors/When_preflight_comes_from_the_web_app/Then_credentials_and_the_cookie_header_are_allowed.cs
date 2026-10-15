using ClassManager.Api.Authentication;
using Microsoft.Net.Http.Headers;

namespace ClassManager.Api.I.Tests.Endpoints.Cors.When_preflight_comes_from_the_web_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_credentials_and_the_cookie_header_are_allowed(ApiFixture fixture)
{
    private const string ExpoWebDevelopmentOrigin = "http://localhost:8081";
    private const string AllowCredentialsValue = "true";

    [Fact]
    public async Task Then_credentials_and_the_cookie_header_are_allowed_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        using var preflightRequest = new HttpRequestMessage(
            HttpMethod.Options, new Uri(AuthenticationRequests.RefreshRoute, UriKind.Relative));
        preflightRequest.Headers.Add(HeaderNames.Origin, ExpoWebDevelopmentOrigin);
        preflightRequest.Headers.Add(HeaderNames.AccessControlRequestMethod, HttpMethod.Post.Method);
        preflightRequest.Headers.Add(
            HeaderNames.AccessControlRequestHeaders, $"{HeaderNames.ContentType},{RefreshTokenCookie.RequestHeaderName}");

        using var response = await client.SendAsync(preflightRequest);

        response.Headers.GetValues(HeaderNames.AccessControlAllowCredentials).ShouldBe([AllowCredentialsValue]);
        string.Join(',', response.Headers.GetValues(HeaderNames.AccessControlAllowHeaders))
            .ShouldContain(RefreshTokenCookie.RequestHeaderName, Case.Insensitive);
    }
}
