using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_ClassPackClassGroup_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClassGroup = await otherBusiness.HttpClient.CreateClassGroupWithInstructorAsync();
        var otherPack = await otherBusiness.HttpClient.CreateClassPackAsync(classGroupIds: [otherClassGroup.Id]);

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.ClassPackClassGroups.AnyAsync(classPackClassGroup => classPackClassGroup.ClassPackId == otherPack.Id)).ShouldBeFalse();
    }
}
