namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_requests_monthly_fees;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Coach.GetAsync(new Uri(ApiRoutes.Fees, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
