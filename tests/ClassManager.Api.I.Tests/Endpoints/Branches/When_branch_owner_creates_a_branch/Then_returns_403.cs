using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_branch_owner_creates_a_branch;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var branchOwner = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.BranchOwner);

        using var response = await branchOwner.PostBranchAsync("DF Valencia");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
