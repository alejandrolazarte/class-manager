using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_member_is_made_brand_owner;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_see_every_branch_of_the_brand(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_see_every_branch_of_the_brand_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var branch = await business.HttpClient.CreateBranchAsync("DF Valencia");
        using var viewer = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.Viewer);
        var viewerMember = (await business.HttpClient.GetTeamAsync()).Members.Single(member => member.Role == BusinessRole.Viewer);

        using (var promoteResponse = await business.HttpClient.PutBrandOwnerAsync(viewerMember.Id))
        {
            promoteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        var branches = await viewer.ListBranchesAsync();
        branches.Select(listedBranch => listedBranch.BusinessId).ShouldBe([business.Business.Id, branch.BusinessId], ignoreOrder: true);
        (await business.HttpClient.GetTeamAsync()).Members.Single(member => member.Id == viewerMember.Id).IsBrandOwner.ShouldBeTrue();
    }
}
