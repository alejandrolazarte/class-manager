using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_a_second_announcement_is_pinned;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_first_is_unpinned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_first_is_unpinned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand("Horarios de verano", null, IsPinned: true));
        var second = await business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand("Cómo pagar la cuota", null));

        (await business.HttpClient.PinAnnouncementAsync(second.Id)).StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var pinned = (await business.HttpClient.ListAnnouncementsAsync())!.Where(announcement => announcement.IsPinned).ToList();
        pinned.Select(announcement => announcement.Id).ShouldBe([second.Id]);
    }
}
