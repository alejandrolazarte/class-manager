using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_entity_of_another_business_is_modified;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_save_is_rejected(ApiFixture fixture)
{
    [Fact]
    public async Task Then_save_is_rejected_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherBusinessClientId = (await otherBusiness.HttpClient.RegisterClientAsync()).Id;
        await using var otherBusinessContext = fixture.CreateDbContext(otherBusiness.Business.Id);
        var otherBusinessClient = await otherBusinessContext.Clients.SingleAsync(client => client.Id == otherBusinessClientId);

        await using var context = fixture.CreateDbContext(business.Business.Id);
        context.Clients.Attach(otherBusinessClient).State = EntityState.Modified;

        await Should.ThrowAsync<InvalidOperationException>(() => context.SaveChangesAsync());
    }
}
