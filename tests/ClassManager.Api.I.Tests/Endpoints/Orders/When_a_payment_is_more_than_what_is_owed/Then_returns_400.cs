using ClassManager.Core.Domain.Products;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_a_payment_is_more_than_what_is_owed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var product = await owner.CreateProductAsync(StockMode.Unlimited);
        var order = await owner.SellAtCounterAsync(null, [OrderRequests.ProductLine(product.Variants[0].Id, 1)], paidAmount: 0m);

        using var response = await owner.PostOrderPaymentAsync(order.Id, order.Total + 1);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await owner.ListOrdersAsync(owing: true)).Single().Balance.ShouldBe(order.Total);
    }
}
