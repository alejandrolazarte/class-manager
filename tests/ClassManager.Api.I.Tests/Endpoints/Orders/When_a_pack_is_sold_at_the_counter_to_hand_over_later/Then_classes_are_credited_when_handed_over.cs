namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_a_pack_is_sold_at_the_counter_to_hand_over_later;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_classes_are_credited_when_handed_over(ApiFixture fixture)
{
    [Fact]
    public async Task Then_classes_are_credited_when_handed_over_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var client = await owner.RegisterClientAsync();
        var classPack = await owner.CreateClassPackAsync();
        var order = await owner.SellAtCounterAsync(client.Id, [OrderRequests.PackLine(classPack.Id)], isDelivered: false, paidAmount: 30m);
        (await owner.GetClassBalanceAsync(client.Id))!.AvailableClasses.ShouldBe(0);

        (await owner.PutDeliveredAsync(order.Id)).EnsureSuccessStatusCode();

        (await owner.GetClassBalanceAsync(client.Id))!.AvailableClasses.ShouldBe(4);
    }
}
