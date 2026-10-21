namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_member_deletes_a_payment_someone_else_recorded;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedOrderCollectorScenarioAsync();
        var order = await scenario.Owner.SellAtCounterAsync(scenario.OwnClientId, [OrderRequests.ProductLine(scenario.ProductVariantId, 1)]);

        using var response = await scenario.OrderCollector.DeleteOrderPaymentAsync(order.Id, order.Payments.Single().Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await scenario.Owner.ListOrdersAsync()).Single().Payments.ShouldNotBeEmpty();
    }
}
