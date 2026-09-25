using Microsoft.Net.Http.Headers;

namespace ClassManager.Api.I.Tests.Endpoints.Cors.When_preflight_comes_from_allowed_origin;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_origin_and_authorization_header_are_allowed(ApiFixture fixture)
{
    private const string ExpoWebDevelopmentOrigin = "http://localhost:8081";

    [Fact]
    public async Task Then_origin_and_authorization_header_are_allowed_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        using var preflightRequest = new HttpRequestMessage(HttpMethod.Options, new Uri(ApiRoutes.Clients, UriKind.Relative));
        preflightRequest.Headers.Add(HeaderNames.Origin, ExpoWebDevelopmentOrigin);
        preflightRequest.Headers.Add(HeaderNames.AccessControlRequestMethod, HttpMethod.Post.Method);
        preflightRequest.Headers.Add(HeaderNames.AccessControlRequestHeaders, $"{HeaderNames.ContentType},{HeaderNames.Authorization}");

        using var response = await client.SendAsync(preflightRequest);

        response.Headers.GetValues(HeaderNames.AccessControlAllowOrigin).ShouldBe([ExpoWebDevelopmentOrigin]);
        string.Join(',', response.Headers.GetValues(HeaderNames.AccessControlAllowHeaders)).ShouldContain(HeaderNames.Authorization, Case.Insensitive);
    }
}
