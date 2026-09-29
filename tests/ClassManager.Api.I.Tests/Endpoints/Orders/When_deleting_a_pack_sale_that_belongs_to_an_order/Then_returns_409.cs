namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_deleting_a_pack_sale_that_belongs_to_an_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();
        var order = await business.HttpClient.SellAtCounterAsync(client.Id, [OrderRequests.PackLine(classPack.Id)]);

        using var response = await business.HttpClient.DeleteAsync(
            new Uri($"{ApiRoutes.ClassPackPurchases}/{order.Lines[0].ClassPackPurchaseId}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
