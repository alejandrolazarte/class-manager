namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_selling_a_pack_at_the_counter;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_classes_are_credited(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_classes_are_credited_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();

        var order = await business.HttpClient.SellAtCounterAsync(client.Id, [OrderRequests.PackLine(classPack.Id)]);

        var balance = await business.HttpClient.GetClassBalanceAsync(client.Id);
        balance!.AvailableClasses.ShouldBe(4);
        order.Lines.Single().ClassPackPurchaseId.ShouldBe(balance.Purchases.Single().Id);
    }
}
