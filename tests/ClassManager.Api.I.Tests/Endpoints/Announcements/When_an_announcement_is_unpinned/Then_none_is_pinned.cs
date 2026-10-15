using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_an_announcement_is_unpinned;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_none_is_pinned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_none_is_pinned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var announcement = await business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand("Horarios de verano", null, IsPinned: true));

        (await business.HttpClient.UnpinAnnouncementAsync(announcement.Id)).StatusCode.ShouldBe(HttpStatusCode.NoContent);

        (await business.HttpClient.ListAnnouncementsAsync())!.ShouldNotContain(listed => listed.IsPinned);
    }
}
