using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_an_announcement_is_published_pinned;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_replaces_the_pinned_one(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_replaces_the_pinned_one_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand("Horarios de verano", null, IsPinned: true));

        var latest = await business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand("Cambia el horario", null, IsPinned: true));

        var pinned = (await business.HttpClient.ListAnnouncementsAsync())!.Where(announcement => announcement.IsPinned).ToList();
        pinned.Select(announcement => announcement.Id).ShouldBe([latest.Id]);
    }
}
