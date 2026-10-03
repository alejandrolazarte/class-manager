namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_selling_a_pack_from_the_client_card;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_listed_in_orders(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_listed_in_orders_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();

        var purchase = await business.HttpClient.SellClassPackAsync(client.Id, classPack.Id);

        var orders = await business.HttpClient.ListOrdersAsync();
        orders.Single().Lines.Single().ClassPackPurchaseId.ShouldBe(purchase.Id);
        orders.Single().Total.ShouldBe(purchase.Price);
    }
}
