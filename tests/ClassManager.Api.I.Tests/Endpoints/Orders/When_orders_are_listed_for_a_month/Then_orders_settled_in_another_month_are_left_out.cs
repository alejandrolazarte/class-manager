namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_orders_are_listed_for_a_month;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_orders_settled_in_another_month_are_left_out(ApiFixture fixture)
{
    private const string NextMonth = "2026-10";

    [Fact]
    public async Task Then_orders_settled_in_another_month_are_left_out_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var classPack = await owner.CreateClassPackAsync();
        var paidOrder = await owner.SellAtCounterAsync(scenario.ClientId, [OrderRequests.PackLine(classPack.Id)]);
        var owedOrder = await owner.SellAtCounterAsync(scenario.ClientId, [OrderRequests.PackLine(classPack.Id)], paidAmount: 0m);

        var orders = await owner.ListOrdersAsync(month: NextMonth);

        orders.Select(order => order.Id).ShouldBe([owedOrder.Id]);
        (await owner.ListOrdersAsync(month: FeeRequests.CurrentMonth)).Select(order => order.Id).ShouldBe([owedOrder.Id, paidOrder.Id], ignoreOrder: true);
    }
}
