namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_member_sells_to_a_student_that_is_not_theirs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedOrderCollectorScenarioAsync();

        using var response = await scenario.OrderCollector.PostCounterSaleAsync(
            scenario.OtherClientId, [OrderRequests.ProductLine(scenario.ProductVariantId, 1)]);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
