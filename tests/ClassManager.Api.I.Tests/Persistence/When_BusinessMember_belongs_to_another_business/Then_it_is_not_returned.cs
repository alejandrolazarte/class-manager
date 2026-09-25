using ClassManager.Core.Domain.Businesses;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_BusinessMember_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherBusinessMember = BusinessMember.CreateOwner(otherBusiness.Business.Id, Guid.CreateVersion7());
        await using (var otherBusinessContext = fixture.CreateDbContext(otherBusiness.Business.Id))
        {
            otherBusinessContext.BusinessMembers.Add(otherBusinessMember);
            await otherBusinessContext.SaveChangesAsync();
        }

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.BusinessMembers.AnyAsync(member => member.Id == otherBusinessMember.Id)).ShouldBeFalse();
    }
}
