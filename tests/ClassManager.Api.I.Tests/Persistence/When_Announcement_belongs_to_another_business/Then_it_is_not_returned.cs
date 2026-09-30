using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_Announcement_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherAnnouncement = await otherBusiness.HttpClient.PublishAnnouncementAsync();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.Announcements.AnyAsync(announcement => announcement.Id == otherAnnouncement.Id)).ShouldBeFalse();
    }
}
