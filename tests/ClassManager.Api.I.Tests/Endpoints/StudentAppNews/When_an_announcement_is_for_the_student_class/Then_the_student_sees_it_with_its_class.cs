using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Announcements;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_an_announcement_is_for_the_student_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_sees_it_with_its_class(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_sees_it_with_its_class_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand(
            "Cambia el horario",
            null,
            AnnouncementCategory.Important,
            [scenario.Instructors.InstructorClassGroup.Id, scenario.Instructors.OtherClassGroup.Id]));

        var news = await scenario.Student.GetStudentAppNewsAsync();

        var announcement = news!.Items.Single(item => item.Kind == StudentAppNewsKind.Announcement);
        announcement.Category.ShouldBe(AnnouncementCategory.Important);
        announcement.AudienceClassNames.ShouldBe([InstructorScenario.InstructorClassGroupName]);
    }
}
