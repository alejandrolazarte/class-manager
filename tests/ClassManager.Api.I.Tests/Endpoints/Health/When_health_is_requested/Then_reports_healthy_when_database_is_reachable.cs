namespace ClassManager.Api.I.Tests.Endpoints.Health.When_health_is_requested;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_reports_healthy_when_database_is_reachable(ApiFixture fixture)
{
    [Fact]
    public async Task Then_reports_healthy_when_database_is_reachable_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();

        using var response = await client.GetAsync(new Uri(ApiRoutes.Health, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
