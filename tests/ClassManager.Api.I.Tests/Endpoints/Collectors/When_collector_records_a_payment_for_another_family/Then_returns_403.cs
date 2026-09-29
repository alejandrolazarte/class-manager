namespace ClassManager.Api.I.Tests.Endpoints.Collectors.When_collector_records_a_payment_for_another_family;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedCollectorScenarioAsync();

        using var response = await scenario.Collector.PostPaymentAsync(scenario.OtherFamilyId);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
