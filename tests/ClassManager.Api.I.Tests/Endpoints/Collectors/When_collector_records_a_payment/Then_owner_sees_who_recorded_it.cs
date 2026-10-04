namespace ClassManager.Api.I.Tests.Endpoints.Collectors.When_collector_records_a_payment;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_owner_sees_who_recorded_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_owner_sees_who_recorded_it_Run()
    {
        var scenario = await fixture.SeedCollectorScenarioAsync();
        var collectorUserId = (await scenario.Collector.GetCurrentMemberAsync()).UserId;

        using var response = await scenario.Collector.PostPaymentAsync(scenario.CollectorClientId);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var payments = await scenario.Coaches.Business.HttpClient.GetClientPaymentsAsync(scenario.CollectorClientId);
        payments!.Single().RecordedByUserId.ShouldBe(collectorUserId);
    }
}
