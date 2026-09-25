namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_refreshing;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_old_refresh_token_is_revoked(ApiFixture fixture)
{
    [Fact]
    public async Task Then_old_refresh_token_is_revoked_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var signUpTokens = await client.SignUpAsync(AuthenticationRequests.SignUpCommand());
        using var refreshResponse = await client.PostRefreshAsync(signUpTokens.RefreshToken);
        var refreshedTokens = await refreshResponse.ReadTokensAsync();

        using var reuseResponse = await client.PostRefreshAsync(signUpTokens.RefreshToken);

        refreshResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        refreshedTokens.RefreshToken.ShouldNotBe(signUpTokens.RefreshToken);
        reuseResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
