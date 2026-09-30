namespace ClassManager.Api.I.Tests.Endpoints.FamilyNews.When_family_opens_the_news;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_nothing_is_unread(ApiFixture fixture)
{
    [Fact]
    public async Task Then_nothing_is_unread_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        await scenario.Coaches.Business.HttpClient.PublishAnnouncementAsync();

        using var seen = await scenario.Family.PutFamilyNewsSeenAsync();

        seen.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var news = await scenario.Family.GetFamilyNewsAsync();
        news!.UnreadCount.ShouldBe(0);
        news.Items.ShouldAllBe(item => !item.IsUnread);
    }
}
