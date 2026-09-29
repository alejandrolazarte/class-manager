namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_member_changes_their_own_brand_owner_status;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var ownMember = (await business.HttpClient.GetTeamAsync()).Members.Single(member => member.IsCurrentUser);

        using var response = await business.HttpClient.DeleteAsync(
            new Uri($"{ApiRoutes.Members}/{ownMember.Id}{ApiRoutes.BrandOwnerSegment}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
