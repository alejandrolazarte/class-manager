using ClassManager.Core.Domain.Clients;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_entity_is_added;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_tenant_id_is_stamped(ApiFixture fixture)
{
    [Fact]
    public async Task Then_tenant_id_is_stamped_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var phoneNumber = PhoneNumber.Create(ApiRequests.ClientPhoneNumber, business.Business.DefaultCountryCallingCode).Value!;
        var client = Client.Create(ApiRequests.ClientFullName, phoneNumber, null, null, BusinessApiFactory.Now).Value!;
        await using (var context = fixture.CreateDbContext(business.Business.Id))
        {
            context.Clients.Add(client);
            await context.SaveChangesAsync();
        }

        await using var readContext = fixture.CreateDbContext(business.Business.Id);

        (await readContext.Clients.SingleAsync(saved => saved.Id == client.Id)).TenantId.ShouldBe(business.Business.Id);
    }
}
