using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_member_changes_their_own_role;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var team = await business.HttpClient.GetTeamAsync();
        var ownMember = team.Members.Single(member => member.IsCurrentUser);

        using var response = await business.HttpClient.PutMemberAsync(ownMember.Id, BusinessRole.Viewer);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
