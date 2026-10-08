using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_the_school_publishes_an_announcement;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_sees_it_unread(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_sees_it_unread_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync();

        var news = await scenario.Student.GetStudentAppNewsAsync();

        var announcement = news!.Items.Single(item => item.Kind == StudentAppNewsKind.Announcement);
        announcement.Title.ShouldBe(NewsRequests.AnnouncementTitle);
        announcement.Body.ShouldBe(NewsRequests.AnnouncementBody);
        announcement.IsUnread.ShouldBeTrue();
        news.UnreadCount.ShouldBe(1);
    }
}
