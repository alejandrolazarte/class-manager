using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Families.When_family_refreshes_the_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_stays_a_family_session(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_stays_a_family_session_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostRefreshAsync(scenario.Tokens.RefreshToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var refreshed = await response.ReadTokensAsync();
        refreshed.Kind.ShouldBe(AccountKinds.Family);
        using var family = fixture.CreateClientWithToken(refreshed.AccessToken);
        (await family.GetFamilyHomeAsync()).ShouldNotBeNull();
    }
}
