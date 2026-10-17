using ClassManager.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_User_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherUser = User.Create(otherBusiness.Business.Id, "Ana Pérez", BusinessApiFactory.Now);
        await using (var otherBusinessContext = fixture.CreateDbContext(otherBusiness.Business.Id))
        {
            otherBusinessContext.Users.Add(otherUser);
            await otherBusinessContext.SaveChangesAsync();
        }

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.Users.AnyAsync(user => user.Id == otherUser.Id)).ShouldBeFalse();
    }
}
