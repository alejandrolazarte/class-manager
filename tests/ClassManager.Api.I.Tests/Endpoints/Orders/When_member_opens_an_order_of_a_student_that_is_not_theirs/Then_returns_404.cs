namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_member_opens_an_order_of_a_student_that_is_not_theirs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedOrderCollectorScenarioAsync();
        var order = await scenario.Owner.SellAtCounterAsync(scenario.OtherClientId, [OrderRequests.ProductLine(scenario.ProductVariantId, 1)]);

        using var response = await scenario.OrderCollector.GetAsync(new Uri($"{ApiRoutes.Orders}/{order.Id}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
