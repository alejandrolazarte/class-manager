using ClassManager.Core.Domain.Clients;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_ClientAccount_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClient = await otherBusiness.HttpClient.RegisterClientAsync();
        var otherAccount = ClientAccount.Create(otherClient.Id, Guid.CreateVersion7(), BusinessApiFactory.Now);
        await using (var otherBusinessContext = fixture.CreateDbContext(otherBusiness.Business.Id))
        {
            otherBusinessContext.ClientAccounts.Add(otherAccount);
            await otherBusinessContext.SaveChangesAsync();
        }

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.ClientAccounts.AnyAsync(account => account.Id == otherAccount.Id)).ShouldBeFalse();
    }
}
