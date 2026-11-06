namespace ClassManager.Api.I.Tests.Endpoints.OnlineOrderPayments.When_another_family_pays_the_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        using var scenario = await fixture.SeedOnlineOrderScenarioAsync();
        using var otherScenario = await fixture.SeedOnlineOrderScenarioAsync();

        using var response = await otherScenario.Family.PostOrderCheckoutAsync(scenario.Order.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
