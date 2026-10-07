using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_a_removed_brand_owner_is_made_brand_owner_again;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_see_every_branch_again(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_see_every_branch_again_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var branch = await business.HttpClient.CreateBranchAsync("DF Valencia");
        using var viewer = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.Viewer);
        var viewerMember = (await business.HttpClient.GetTeamAsync()).Members.Single(member => member.Role == BusinessRole.Viewer);
        (await business.HttpClient.PutBrandOwnerAsync(viewerMember.Id)).EnsureSuccessStatusCode();
        (await business.HttpClient.DeleteBrandOwnerAsync(viewerMember.Id)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.PutBrandOwnerAsync(viewerMember.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await viewer.ListBranchesAsync()).Select(listed => listed.BusinessId).ShouldBe([business.Business.Id, branch.BusinessId], ignoreOrder: true);
    }
}
