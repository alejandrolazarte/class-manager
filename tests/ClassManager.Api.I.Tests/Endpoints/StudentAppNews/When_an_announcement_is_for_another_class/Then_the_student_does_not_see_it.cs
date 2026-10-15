using ClassManager.Core.UseCases.Announcements;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_an_announcement_is_for_another_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_does_not_see_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_does_not_see_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand(
            "Cambia el horario", null, ClassGroupIds: [scenario.Instructors.OtherClassGroup.Id]));

        var news = await scenario.Student.GetStudentAppNewsAsync();

        news!.Items.ShouldNotContain(item => item.Kind == StudentAppNewsKind.Announcement);
        news.UnreadCount.ShouldBe(0);
    }
}
