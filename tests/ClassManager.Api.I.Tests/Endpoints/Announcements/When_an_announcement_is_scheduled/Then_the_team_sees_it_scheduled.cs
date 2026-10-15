using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_an_announcement_is_scheduled;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_scheduled(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_sees_it_scheduled_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var publishOn = BusinessApiFactory.Now.AddDays(2);
        await business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand("Feriado del 8 de diciembre", null, PublishOn: publishOn));

        var announcement = (await business.HttpClient.ListAnnouncementsAsync())!.Single();

        announcement.IsScheduled.ShouldBeTrue();
        announcement.PublishedAt.ShouldBe(publishOn);
    }
}
