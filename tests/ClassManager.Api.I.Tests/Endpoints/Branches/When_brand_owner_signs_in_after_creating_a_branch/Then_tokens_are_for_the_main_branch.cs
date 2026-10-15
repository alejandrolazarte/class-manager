using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_brand_owner_signs_in_after_creating_a_branch;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_tokens_are_for_the_main_branch(ApiFixture fixture)
{
    private const int BranchCount = 2;

    [Fact]
    public async Task Then_tokens_are_for_the_main_branch_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        var tokens = await client.SignUpAsync(fixture.ApiFactory.EmailTransport, command);
        using var ownerClient = fixture.CreateClientWithToken(tokens.AccessToken);
        var mainBusinessId = (await ownerClient.GetCurrentMemberAsync()).BusinessId;
        await fixture.ChangePlanAsync(mainBusinessId, PlanCodes.Pro);
        for (var branchNumber = 1; branchNumber <= BranchCount; branchNumber++)
        {
            await ownerClient.CreateBranchAsync($"Branch {branchNumber}");
        }

        var signInTokens = await client.SignInAsync(command.Email!);

        using var signedInClient = fixture.CreateClientWithToken(signInTokens.AccessToken);
        (await signedInClient.GetCurrentMemberAsync()).BusinessId.ShouldBe(mainBusinessId);
    }
}
