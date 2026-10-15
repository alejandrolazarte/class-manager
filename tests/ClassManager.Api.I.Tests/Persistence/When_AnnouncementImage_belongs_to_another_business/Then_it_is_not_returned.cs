using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_AnnouncementImage_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherAnnouncement = await otherBusiness.HttpClient.PublishAnnouncementAsync();
        var otherImage = (await otherBusiness.HttpClient.AddAnnouncementImageAsync(otherAnnouncement.Id)).Images.Single();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.AnnouncementImages.AnyAsync(image => image.DocumentId == otherImage.Id)).ShouldBeFalse();
    }
}
