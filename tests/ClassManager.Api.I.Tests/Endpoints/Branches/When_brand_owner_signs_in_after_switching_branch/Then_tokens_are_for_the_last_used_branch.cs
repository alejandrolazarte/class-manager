using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_brand_owner_signs_in_after_switching_branch;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_tokens_are_for_the_last_used_branch(ApiFixture fixture)
{
    [Fact]
    public async Task Then_tokens_are_for_the_last_used_branch_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        var tokens = await client.SignUpAsync(fixture.ApiFactory.EmailTransport, command);
        using var ownerClient = fixture.CreateClientWithToken(tokens.AccessToken);
        await fixture.ChangePlanAsync((await ownerClient.GetCurrentMemberAsync()).BusinessId, PlanCodes.Pro);
        var usedBranch = await ownerClient.CreateBranchAsync("Branch used last");
        await ownerClient.CreateBranchAsync("Branch created last");
        await client.SwitchBranchAsync(tokens.RefreshToken, usedBranch.BusinessId);

        var signInTokens = await client.SignInAsync(command.Email!);

        using var signedInClient = fixture.CreateClientWithToken(signInTokens.AccessToken);
        (await signedInClient.GetCurrentMemberAsync()).BusinessId.ShouldBe(usedBranch.BusinessId);
    }
}
