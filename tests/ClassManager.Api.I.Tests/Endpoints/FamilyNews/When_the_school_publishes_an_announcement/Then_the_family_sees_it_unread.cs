using ClassManager.Core.UseCases.Families;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyNews.When_the_school_publishes_an_announcement;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_sees_it_unread(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_family_sees_it_unread_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        await scenario.Coaches.Business.HttpClient.PublishAnnouncementAsync();

        var news = await scenario.Family.GetFamilyNewsAsync();

        var announcement = news!.Items.Single(item => item.Kind == FamilyNewsKind.Announcement);
        announcement.Title.ShouldBe(NewsRequests.AnnouncementTitle);
        announcement.Body.ShouldBe(NewsRequests.AnnouncementBody);
        announcement.IsUnread.ShouldBeTrue();
        news.UnreadCount.ShouldBe(1);
    }
}
