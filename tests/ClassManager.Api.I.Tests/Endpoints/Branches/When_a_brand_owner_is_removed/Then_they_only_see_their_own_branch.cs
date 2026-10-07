using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_a_brand_owner_is_removed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_only_see_their_own_branch(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_only_see_their_own_branch_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.CreateBranchAsync("DF Valencia");
        using var viewer = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.Viewer);
        var viewerMember = (await business.HttpClient.GetTeamAsync()).Members.Single(member => member.Role == BusinessRole.Viewer);
        (await business.HttpClient.PutBrandOwnerAsync(viewerMember.Id)).EnsureSuccessStatusCode();

        (await business.HttpClient.DeleteBrandOwnerAsync(viewerMember.Id)).EnsureSuccessStatusCode();

        (await viewer.ListBranchesAsync()).Select(branch => branch.BusinessId).ShouldBe([business.Business.Id]);
    }
}
