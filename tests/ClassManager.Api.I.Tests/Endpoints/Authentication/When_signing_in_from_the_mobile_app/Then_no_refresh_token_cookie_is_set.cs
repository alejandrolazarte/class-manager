using Microsoft.Net.Http.Headers;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_in_from_the_mobile_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_refresh_token_cookie_is_set(ApiFixture fixture)
{
    [Fact]
    public async Task Then_no_refresh_token_cookie_is_set_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        await client.SignUpAsync(fixture.ApiFactory.EmailTransport, command);

        using var response = await client.PostSignInAsync(command.Email!, AuthenticationRequests.OwnerPassword);

        response.Headers.Contains(HeaderNames.SetCookie).ShouldBeFalse();
        (await response.ReadTokensAsync()).RefreshToken.ShouldNotBeNullOrEmpty();
    }
}
