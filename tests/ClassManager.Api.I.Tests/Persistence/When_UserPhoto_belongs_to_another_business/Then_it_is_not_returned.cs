using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_UserPhoto_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        await otherBusiness.HttpClient.ChangeMyPhotoAsync();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        await using var otherContext = fixture.CreateDbContext(otherBusiness.Business.Id);

        (await otherContext.UserPhotos.CountAsync()).ShouldBe(1);
        (await context.UserPhotos.AnyAsync()).ShouldBeFalse();
    }
}
