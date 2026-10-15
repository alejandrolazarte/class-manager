using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Announcements;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_a_student_answers_an_event_again;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_sees_the_last_answer(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_sees_the_last_answer_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var announcement = await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync(
            new CreateAnnouncementCommand("Muestra de fin de año", null, AnnouncementCategory.Event));
        (await scenario.Student.PutAnnouncementReplyAsync(announcement.Id, isGoing: true)).EnsureSuccessStatusCode();

        (await scenario.Student.PutAnnouncementReplyAsync(announcement.Id, isGoing: false)).EnsureSuccessStatusCode();

        var news = await scenario.Student.GetStudentAppNewsAsync();
        var item = news!.Items.Single(newsItem => newsItem.Kind == StudentAppNewsKind.Announcement);
        item.IsGoing.ShouldBe(false);
        item.AnnouncementId.ShouldBe(announcement.Id);
    }
}
