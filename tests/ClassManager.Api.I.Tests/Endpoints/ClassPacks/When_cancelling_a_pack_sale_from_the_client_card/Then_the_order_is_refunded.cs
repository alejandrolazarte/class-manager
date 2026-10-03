namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_cancelling_a_pack_sale_from_the_client_card;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_order_is_refunded(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_order_is_refunded_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();
        var purchase = await business.HttpClient.SellClassPackAsync(client.Id, classPack.Id);

        using var response = await business.HttpClient.DeleteAsync(
            new Uri($"{ApiRoutes.ClassPackPurchases}/{purchase.Id}", UriKind.Relative));

        response.EnsureSuccessStatusCode();
        var order = (await business.HttpClient.ListOrdersAsync()).Single();
        order.RefundedAmount.ShouldBe(purchase.Price);
        (await business.HttpClient.GetClassBalanceAsync(client.Id))!.Purchases.ShouldBeEmpty();
    }
}
