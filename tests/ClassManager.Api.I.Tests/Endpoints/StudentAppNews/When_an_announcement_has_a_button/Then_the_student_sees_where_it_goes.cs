using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_an_announcement_has_a_button;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_sees_where_it_goes(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_sees_where_it_goes_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand(
            "Inscripciones abiertas", null, Action: AnnouncementAction.Link, ActionUrl: "https://example.com/inscripcion"));

        var news = await scenario.Student.GetStudentAppNewsAsync();

        var item = news!.Items.Single(candidate => candidate.Title == "Inscripciones abiertas");
        item.Action.ShouldBe(AnnouncementAction.Link);
        item.ActionUrl.ShouldBe("https://example.com/inscripcion");
    }
}
