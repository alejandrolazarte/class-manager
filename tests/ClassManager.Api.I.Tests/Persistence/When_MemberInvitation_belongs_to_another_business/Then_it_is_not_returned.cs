using ClassManager.Core.Domain.Businesses;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_MemberInvitation_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherInvitation = MemberInvitation.Create(
            "someone@example.com", BusinessRole.Viewer, null, Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"), Guid.CreateVersion7(), BusinessApiFactory.Now).Value!;
        await using (var otherBusinessContext = fixture.CreateDbContext(otherBusiness.Business.Id))
        {
            otherBusinessContext.MemberInvitations.Add(otherInvitation);
            await otherBusinessContext.SaveChangesAsync();
        }

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.MemberInvitations.AnyAsync(invitation => invitation.Id == otherInvitation.Id)).ShouldBeFalse();
    }
}
