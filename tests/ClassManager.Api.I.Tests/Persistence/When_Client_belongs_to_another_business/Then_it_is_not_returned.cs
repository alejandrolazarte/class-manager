using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_Client_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherBusinessClient = await otherBusiness.HttpClient.RegisterClientAsync();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.Clients.AnyAsync(client => client.Id == otherBusinessClient.Id)).ShouldBeFalse();
    }
}
