using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_refunding_an_unused_pack;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_classes_are_removed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_classes_are_removed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();
        var order = await business.HttpClient.SellAtCounterAsync(client.Id, [OrderRequests.PackLine(classPack.Id)]);

        using var response = await business.HttpClient.PostRefundAsync(order.Id, new RefundLine(order.Lines[0].Id, null, false));

        (await response.Content.ReadFromJsonAsync<OrderResponse>(ApiRequests.JsonOptions))!.RefundedAmount.ShouldBe(80m);
        (await business.HttpClient.GetClassBalanceAsync(client.Id))!.Purchases.ShouldBeEmpty();
    }
}
