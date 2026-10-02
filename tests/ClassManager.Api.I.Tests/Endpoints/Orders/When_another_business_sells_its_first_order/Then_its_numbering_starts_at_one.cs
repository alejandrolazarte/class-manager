namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_another_business_sells_its_first_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_numbering_starts_at_one(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_numbering_starts_at_one_Run()
    {
        var businessA = await fixture.SeedBusinessAsync();
        var clientA = await businessA.HttpClient.RegisterClientAsync();
        var classPackA = await businessA.HttpClient.CreateClassPackAsync();
        await businessA.HttpClient.SellAtCounterAsync(clientA.Id, [OrderRequests.PackLine(classPackA.Id)]);
        var businessB = await fixture.SeedBusinessAsync();
        var clientB = await businessB.HttpClient.RegisterClientAsync();
        var classPackB = await businessB.HttpClient.CreateClassPackAsync();

        var orderB = await businessB.HttpClient.SellAtCounterAsync(clientB.Id, [OrderRequests.PackLine(classPackB.Id)]);

        orderB.Number.ShouldBe(1);
    }
}
