using ClassManager.Core.Domain.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_CustomRole_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherRole = await fixture.SeedCustomRoleAsync(otherBusiness.Business.Id, [Permissions.Business.View]);

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.CustomRoles.AnyAsync(role => role.Id == otherRole.Id)).ShouldBeFalse();
    }
}
