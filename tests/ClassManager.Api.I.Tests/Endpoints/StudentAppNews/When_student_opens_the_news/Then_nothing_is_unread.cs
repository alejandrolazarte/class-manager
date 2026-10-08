namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_student_opens_the_news;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_nothing_is_unread(ApiFixture fixture)
{
    [Fact]
    public async Task Then_nothing_is_unread_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync();

        using var seen = await scenario.Student.PutStudentAppNewsSeenAsync();

        seen.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var news = await scenario.Student.GetStudentAppNewsAsync();
        news!.UnreadCount.ShouldBe(0);
        news.Items.ShouldAllBe(item => !item.IsUnread);
    }
}
