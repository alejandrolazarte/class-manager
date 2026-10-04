namespace ClassManager.Api.I.Tests.Endpoints.Collectors.When_collector_lists_monthly_fees;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_students_are_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_students_are_listed_Run()
    {
        var scenario = await fixture.SeedCollectorScenarioAsync();

        var fees = await scenario.Collector.GetMonthlyFeesAsync();

        fees!.Clients.Select(client => client.ClientId).ShouldBe([scenario.CollectorClientId]);
    }
}
