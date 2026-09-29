using ClassManager.Core.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_OrderLine_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherProduct = await otherBusiness.HttpClient.CreateProductAsync(StockMode.Unlimited);
        var otherOrder = await otherBusiness.HttpClient.SellAtCounterAsync(null, [OrderRequests.ProductLine(otherProduct.Variants[0].Id, 1)]);

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.OrderLines.AnyAsync(line => line.OrderId == otherOrder.Id)).ShouldBeFalse();
    }
}
