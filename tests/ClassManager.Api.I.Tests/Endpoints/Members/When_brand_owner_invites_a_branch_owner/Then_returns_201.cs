using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_brand_owner_invites_a_branch_owner;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_201(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_201_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostInvitationAsync(MemberRequests.UniqueInviteeEmail(), BusinessRole.BranchOwner);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
