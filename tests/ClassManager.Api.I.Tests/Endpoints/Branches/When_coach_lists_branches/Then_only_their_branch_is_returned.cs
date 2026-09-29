namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_coach_lists_branches;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_branch_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_branch_is_returned_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();
        await scenario.Business.HttpClient.CreateBranchAsync("DF Valencia");

        var branches = await scenario.Coach.ListBranchesAsync();

        branches.Select(branch => branch.BusinessId).ShouldBe([scenario.Business.Business.Id]);
    }
}
