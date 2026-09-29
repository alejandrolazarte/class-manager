using ClassManager.Core.Domain.Clients;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_ClientInvitation_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClient = await otherBusiness.HttpClient.RegisterClientAsync();
        var otherInvitation = ClientInvitation.Create(
            otherClient.Id, "family@example.com", new string('a', 64), Guid.CreateVersion7(), BusinessApiFactory.Now).Value!;
        await using (var otherBusinessContext = fixture.CreateDbContext(otherBusiness.Business.Id))
        {
            otherBusinessContext.ClientInvitations.Add(otherInvitation);
            await otherBusinessContext.SaveChangesAsync();
        }

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.ClientInvitations.AnyAsync(invitation => invitation.Id == otherInvitation.Id)).ShouldBeFalse();
    }
}
