namespace ClassManager.Api.I.Tests.Endpoints.Collectors.When_collector_reads_payments_of_another_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedCollectorScenarioAsync();

        using var response = await scenario.Collector.GetAsync(
            new Uri($"{ApiRoutes.Clients}/{scenario.OtherClientId}{ApiRoutes.PaymentsSegment}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
