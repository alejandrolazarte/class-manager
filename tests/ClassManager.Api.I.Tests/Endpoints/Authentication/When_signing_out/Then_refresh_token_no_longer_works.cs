using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_out;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_refresh_token_no_longer_works(ApiFixture fixture)
{
    [Fact]
    public async Task Then_refresh_token_no_longer_works_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var tokens = await client.SignUpAsync(AuthenticationRequests.SignUpCommand());
        using var signOutResponse = await client.PostSignOutAsync(tokens.RefreshToken);
        using var repeatedSignOutResponse = await client.PostSignOutAsync(tokens.RefreshToken);

        using var refreshResponse = await client.PostRefreshAsync(tokens.RefreshToken);

        signOutResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        repeatedSignOutResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        refreshResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await refreshResponse.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(AuthenticationErrorCodes.InvalidRefreshToken);
    }
}
