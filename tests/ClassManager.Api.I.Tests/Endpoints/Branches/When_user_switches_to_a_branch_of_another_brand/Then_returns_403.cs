namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_user_switches_to_a_branch_of_another_brand;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var tokens = await client.SignUpAsync(AuthenticationRequests.SignUpCommand());
        var otherBusiness = await fixture.SeedBusinessAsync();

        using var response = await client.PostSwitchBranchAsync(tokens.RefreshToken, otherBusiness.Business.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
