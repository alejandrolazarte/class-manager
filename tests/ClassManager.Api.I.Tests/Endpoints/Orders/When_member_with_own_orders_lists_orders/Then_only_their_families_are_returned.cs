namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_member_with_own_orders_lists_orders;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_families_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_families_are_returned_Run()
    {
        var scenario = await fixture.SeedOrderCollectorScenarioAsync();
        var owner = scenario.Owner;
        var ownFamilyOrder = await owner.SellAtCounterAsync(
            scenario.OwnFamilyId, [OrderRequests.ProductLine(scenario.ProductVariantId, 1)]);
        await owner.SellAtCounterAsync(scenario.OtherFamilyId, [OrderRequests.ProductLine(scenario.ProductVariantId, 1)]);
        await owner.SellAtCounterAsync(null, [OrderRequests.ProductLine(scenario.ProductVariantId, 1)]);

        var orders = await scenario.OrderCollector.ListOrdersAsync();

        orders.Single().Id.ShouldBe(ownFamilyOrder.Id);
    }
}
