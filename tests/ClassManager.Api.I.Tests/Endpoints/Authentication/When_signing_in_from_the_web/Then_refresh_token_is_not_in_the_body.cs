using System.Text.Json;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_in_from_the_web;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_refresh_token_is_not_in_the_body(ApiFixture fixture)
{
    private const string AccessTokenProperty = "accessToken";
    private const string RefreshTokenProperty = "refreshToken";

    [Fact]
    public async Task Then_refresh_token_is_not_in_the_body_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();

        using var response = await client.SignUpAndSignInFromWebAsync(fixture.ApiFactory.EmailTransport);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.TryGetProperty(RefreshTokenProperty, out _).ShouldBeFalse();
        body.GetProperty(AccessTokenProperty).GetString().ShouldNotBeNullOrEmpty();
    }
}
