using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_ClassPackPurchase_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClient = await otherBusiness.HttpClient.RegisterClientAsync();
        var otherPack = await otherBusiness.HttpClient.CreateClassPackAsync();
        var otherPurchase = await otherBusiness.HttpClient.SellClassPackAsync(otherClient.Id, otherPack.Id);

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.ClassPackPurchases.AnyAsync(purchase => purchase.Id == otherPurchase.Id)).ShouldBeFalse();
    }
}
