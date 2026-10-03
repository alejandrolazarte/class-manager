using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_brand_owner_switches_to_a_new_branch;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_access_token_is_for_that_branch(ApiFixture fixture)
{
    [Fact]
    public async Task Then_access_token_is_for_that_branch_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var tokens = await client.SignUpAsync(AuthenticationRequests.SignUpCommand());
        using var ownerClient = fixture.CreateClientWithToken(tokens.AccessToken);
        await fixture.ChangePlanAsync((await ownerClient.GetCurrentMemberAsync()).BusinessId, PlanCodes.Pro);
        var branch = await ownerClient.CreateBranchAsync("DF Valencia");

        var switchedTokens = await client.SwitchBranchAsync(tokens.RefreshToken, branch.BusinessId);

        using var branchClient = fixture.CreateClientWithToken(switchedTokens.AccessToken);
        var member = await branchClient.GetCurrentMemberAsync();
        member.BusinessId.ShouldBe(branch.BusinessId);
        member.IsBrandOwner.ShouldBeTrue();
        member.BranchRole.ShouldBeNull();
    }
}
