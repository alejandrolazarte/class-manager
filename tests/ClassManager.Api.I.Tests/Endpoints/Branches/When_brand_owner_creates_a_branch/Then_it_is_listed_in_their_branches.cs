namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_brand_owner_creates_a_branch;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_listed_in_their_branches(ApiFixture fixture)
{
    private const string BranchName = "DF Tenerife";

    [Fact]
    public async Task Then_it_is_listed_in_their_branches_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        var branch = await business.HttpClient.CreateBranchAsync(BranchName);

        var branches = await business.HttpClient.ListBranchesAsync();
        branches.Select(listedBranch => listedBranch.BusinessId).ShouldBe([business.Business.Id, branch.BusinessId], ignoreOrder: true);
        branches.Single(listedBranch => listedBranch.IsCurrent).BusinessId.ShouldBe(business.Business.Id);
    }
}
