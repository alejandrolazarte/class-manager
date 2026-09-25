using Microsoft.Net.Http.Headers;

namespace ClassManager.Api.I.Tests.Endpoints.Cors.When_preflight_comes_from_unknown_origin;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_origin_is_not_allowed(ApiFixture fixture)
{
    private const string UnknownOrigin = "https://attacker.example";

    [Fact]
    public async Task Then_origin_is_not_allowed_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        using var preflightRequest = new HttpRequestMessage(HttpMethod.Options, new Uri(ApiRoutes.Clients, UriKind.Relative));
        preflightRequest.Headers.Add(HeaderNames.Origin, UnknownOrigin);
        preflightRequest.Headers.Add(HeaderNames.AccessControlRequestMethod, HttpMethod.Post.Method);

        using var response = await client.SendAsync(preflightRequest);

        response.Headers.Contains(HeaderNames.AccessControlAllowOrigin).ShouldBeFalse();
    }
}
