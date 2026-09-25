namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_reusing_rotated_refresh_token;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_all_sessions_are_revoked(ApiFixture fixture)
{
    [Fact]
    public async Task Then_all_sessions_are_revoked_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        var firstSession = await client.SignUpAsync(command);
        var secondSession = await client.SignInAsync(command.Email!);
        using var rotationResponse = await client.PostRefreshAsync(firstSession.RefreshToken);
        var rotatedFirstSession = await rotationResponse.ReadTokensAsync();
        using var reuseResponse = await client.PostRefreshAsync(firstSession.RefreshToken);

        using var secondSessionResponse = await client.PostRefreshAsync(secondSession.RefreshToken);

        secondSessionResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        reuseResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        using var rotatedFirstSessionResponse = await client.PostRefreshAsync(rotatedFirstSession.RefreshToken);
        rotatedFirstSessionResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
