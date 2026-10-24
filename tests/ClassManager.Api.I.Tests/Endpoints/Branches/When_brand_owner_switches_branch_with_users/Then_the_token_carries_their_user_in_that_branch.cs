using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_brand_owner_switches_branch_with_users;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_token_carries_their_user_in_that_branch(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_token_carries_their_user_in_that_branch_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var tokens = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(tokens.AccessToken);
        await fixture.ChangePlanAsync((await owner.GetCurrentMemberAsync()).BusinessId, PlanCodes.Enterprise);
        var branch = await owner.CreateBranchAsync("DF Barcelona");

        var switchedTokens = await anonymous.SwitchBranchAsync(tokens.RefreshToken, branch.BusinessId);

        var branchUserId = await fixture.FindUserIdByAccountAsync(branch.BusinessId, UserRows.AccountIdOf(tokens.AccessToken));
        (UserRows.UserIdOf(switchedTokens.AccessToken), UserRows.UserIdOf(switchedTokens.AccessToken) == UserRows.UserIdOf(tokens.AccessToken))
            .ShouldBe((branchUserId, false));
    }
}
