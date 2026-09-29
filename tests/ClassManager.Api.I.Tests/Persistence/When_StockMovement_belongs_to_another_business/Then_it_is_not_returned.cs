using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_StockMovement_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherProduct = await otherBusiness.HttpClient.RestockAsync(await otherBusiness.HttpClient.CreateProductAsync(), 2);

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.StockMovements.AnyAsync(movement => movement.ProductVariantId == otherProduct.Variants[0].Id)).ShouldBeFalse();
    }
}
