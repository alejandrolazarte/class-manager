namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_a_class_pack_sold_at_the_counter_is_owed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_order_summary_does_not_count_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_order_summary_does_not_count_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var classPack = await owner.CreateClassPackAsync();
        await owner.SellAtCounterAsync(scenario.ClientId, [OrderRequests.PackLine(classPack.Id)], paidAmount: 0m);

        var summary = await owner.GetOrderSummaryAsync();

        summary.Unpaid.ShouldBe(0m);
    }
}
