namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_member_with_own_orders_reads_the_summary;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_students_are_counted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_students_are_counted_Run()
    {
        var scenario = await fixture.SeedOrderCollectorScenarioAsync();
        var owner = scenario.Owner;
        await owner.SellAtCounterAsync(scenario.OwnClientId, [OrderRequests.ProductLine(scenario.ProductVariantId, 1)]);
        await owner.SellAtCounterAsync(scenario.OtherClientId, [OrderRequests.ProductLine(scenario.ProductVariantId, 2)]);
        await owner.SellAtCounterAsync(null, [OrderRequests.ProductLine(scenario.ProductVariantId, 3)]);

        var summary = await scenario.OrderCollector.GetOrderSummaryAsync();

        summary.Collected.ShouldBe(ProductRequests.ProductPrice);
    }
}
