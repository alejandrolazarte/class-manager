namespace ClassManager.Api.I.Tests.Endpoints.Collectors.When_collector_records_a_payment;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_payment_names_their_user(ApiFixture fixture)
{
    private const string CollectorFullName = "Laura Gómez";

    [Fact]
    public async Task Then_the_payment_names_their_user_Run()
    {
        var scenario = await fixture.SeedCollectorScenarioAsync();
        var businessId = scenario.Instructors.Business.Business.Id;
        var collectorUserId = (await scenario.Collector.GetCurrentMemberAsync()).UserId;
        await fixture.RenameUserAsync(businessId, collectorUserId!.Value, CollectorFullName);

        using var response = await scenario.Collector.PostPaymentAsync(scenario.CollectorClientId);

        var payments = await scenario.Instructors.Business.HttpClient.GetClientPaymentsAsync(scenario.CollectorClientId);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        payments!.Single().RecordedByFullName.ShouldBe(CollectorFullName);
    }
}
