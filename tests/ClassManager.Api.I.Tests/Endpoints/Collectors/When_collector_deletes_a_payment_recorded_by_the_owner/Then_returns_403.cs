using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Collectors.When_collector_deletes_a_payment_recorded_by_the_owner;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedCollectorScenarioAsync();
        using var ownerPayment = await scenario.Coaches.Business.HttpClient.PostPaymentAsync(scenario.CollectorFamilyId);
        var payment = await ownerPayment.Content.ReadFromJsonAsync<PaymentResponse>(ApiRequests.JsonOptions);

        using var response = await scenario.Collector.DeletePaymentAsync(payment!.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
