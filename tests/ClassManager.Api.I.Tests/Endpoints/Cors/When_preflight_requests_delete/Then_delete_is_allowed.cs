using Microsoft.Net.Http.Headers;

namespace ClassManager.Api.I.Tests.Endpoints.Cors.When_preflight_requests_delete;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_delete_is_allowed(ApiFixture fixture)
{
    private const string ExpoWebDevelopmentOrigin = "http://localhost:8081";

    [Fact]
    public async Task Then_delete_is_allowed_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        using var preflightRequest = new HttpRequestMessage(HttpMethod.Options, new Uri(ApiRoutes.Payments, UriKind.Relative));
        preflightRequest.Headers.Add(HeaderNames.Origin, ExpoWebDevelopmentOrigin);
        preflightRequest.Headers.Add(HeaderNames.AccessControlRequestMethod, HttpMethod.Delete.Method);

        using var response = await client.SendAsync(preflightRequest);

        string.Join(',', response.Headers.GetValues(HeaderNames.AccessControlAllowMethods)).ShouldContain(HttpMethod.Delete.Method);
    }
}
