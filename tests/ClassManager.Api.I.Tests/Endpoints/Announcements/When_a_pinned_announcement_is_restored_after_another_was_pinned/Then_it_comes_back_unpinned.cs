using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_a_pinned_announcement_is_restored_after_another_was_pinned;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_comes_back_unpinned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_comes_back_unpinned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var deleted = await business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand("Horarios de verano", null, IsPinned: true));
        (await business.HttpClient.DeleteAnnouncementAsync(deleted.Id)).EnsureSuccessStatusCode();
        var pinned = await business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand("Cambia el horario", null, IsPinned: true));

        (await business.HttpClient.RestoreAnnouncementAsync(deleted.Id)).EnsureSuccessStatusCode();

        var announcements = (await business.HttpClient.ListAnnouncementsAsync())!;
        announcements.Single(announcement => announcement.Id == deleted.Id).IsPinned.ShouldBeFalse();
        announcements.Single(announcement => announcement.Id == pinned.Id).IsPinned.ShouldBeTrue();
    }
}
