using ClassManager.Core.Domain.Products;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_selling_more_units_than_in_stock;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var product = await business.HttpClient.RestockAsync(await business.HttpClient.CreateProductAsync(), 1);

        using var response = await business.HttpClient.PostCounterSaleAsync(null, [OrderRequests.ProductLine(product.Variants[0].Id, 2)]);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(ProductErrorCodes.OutOfStock);
    }
}
