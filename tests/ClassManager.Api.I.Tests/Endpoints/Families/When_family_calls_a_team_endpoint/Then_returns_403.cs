namespace ClassManager.Api.I.Tests.Endpoints.Families.When_family_calls_a_team_endpoint;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();

        using var response = await scenario.Family.GetAsync(new Uri($"{ApiRoutes.Clients}/{scenario.FamilyId}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
