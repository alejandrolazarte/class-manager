namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_orders_are_sold_at_the_same_time;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_number_repeats(ApiFixture fixture)
{
    private const int ConcurrentSales = 8;

    [Fact]
    public async Task Then_no_number_repeats_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();

        var orders = await Task.WhenAll(Enumerable.Range(0, ConcurrentSales).Select(_ =>
            business.HttpClient.SellAtCounterAsync(client.Id, [OrderRequests.PackLine(classPack.Id)])));

        orders.Select(order => order.Number).Order().ShouldBe(Enumerable.Range(1, ConcurrentSales));
    }
}
