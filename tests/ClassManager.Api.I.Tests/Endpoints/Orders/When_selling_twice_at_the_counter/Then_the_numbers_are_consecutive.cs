namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_selling_twice_at_the_counter;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_numbers_are_consecutive(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_numbers_are_consecutive_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();

        var first = await business.HttpClient.SellAtCounterAsync(client.Id, [OrderRequests.PackLine(classPack.Id)]);
        var second = await business.HttpClient.SellAtCounterAsync(client.Id, [OrderRequests.PackLine(classPack.Id)]);

        new[] { first.Number, second.Number }.ShouldBe([1, 2]);
    }
}
