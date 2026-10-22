namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_member_deletes_a_payment_they_recorded;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_deleted_Run()
    {
        var scenario = await fixture.SeedOrderCollectorScenarioAsync();
        var order = await scenario.OrderCollector.SellAtCounterAsync(
            scenario.OwnClientId, [OrderRequests.ProductLine(scenario.ProductVariantId, 1)]);

        using var response = await scenario.OrderCollector.DeleteOrderPaymentAsync(order.Id, order.Payments.Single().Id);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await scenario.Owner.ListOrdersAsync()).Single().Payments.ShouldBeEmpty();
    }
}
