using ClassManager.Core.UseCases.Announcements;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_an_announcement_is_scheduled;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_does_not_see_it_yet(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_does_not_see_it_yet_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand(
            "Feriado del 8 de diciembre", null, PublishOn: BusinessApiFactory.Now.AddDays(2)));

        var news = await scenario.Student.GetStudentAppNewsAsync();

        news!.Items.ShouldNotContain(item => item.Kind == StudentAppNewsKind.Announcement);
    }
}
