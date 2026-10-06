using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_class_pack_purchase_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_kept_as_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_kept_as_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        var pack = await business.HttpClient.CreateClassPackAsync();
        var purchase = await business.HttpClient.SellClassPackAsync(client.Id, pack.Id);

        (await business.HttpClient.DeleteAsync(new Uri($"{ApiRoutes.ClassPackPurchases}/{purchase.Id}", UriKind.Relative))).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.ClassPackPurchases.AnyAsync(row => row.Id == purchase.Id)).ShouldBeFalse();
        (await context.ClassPackPurchases
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .SingleAsync(row => row.Id == purchase.Id))
            .DeletedOn.ShouldBe(BusinessApiFactory.Now);
    }
}
