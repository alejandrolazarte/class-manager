namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_session_is_refreshed_after_switching_branch;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_stays_in_that_branch(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_stays_in_that_branch_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var tokens = await client.SignUpAsync(AuthenticationRequests.SignUpCommand());
        using var ownerClient = fixture.CreateClientWithToken(tokens.AccessToken);
        var branch = await ownerClient.CreateBranchAsync("DF Barcelona");
        var switchedTokens = await client.SwitchBranchAsync(tokens.RefreshToken, branch.BusinessId);

        using var refreshResponse = await client.PostRefreshAsync(switchedTokens.RefreshToken);

        var refreshedTokens = await refreshResponse.ReadTokensAsync();
        using var refreshedClient = fixture.CreateClientWithToken(refreshedTokens.AccessToken);
        (await refreshedClient.GetCurrentMemberAsync()).BusinessId.ShouldBe(branch.BusinessId);
    }
}
