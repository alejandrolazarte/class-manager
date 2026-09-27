using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_ClassPack_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherPack = await otherBusiness.HttpClient.CreateClassPackAsync();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.ClassPacks.AnyAsync(classPack => classPack.Id == otherPack.Id)).ShouldBeFalse();
    }
}
