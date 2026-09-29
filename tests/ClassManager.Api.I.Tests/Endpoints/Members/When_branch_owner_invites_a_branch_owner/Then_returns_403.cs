using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_branch_owner_invites_a_branch_owner;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var branchOwner = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.BranchOwner);

        using var response = await branchOwner.PostInvitationAsync(MemberRequests.UniqueInviteeEmail(), BusinessRole.BranchOwner);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
