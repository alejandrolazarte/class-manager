using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_a_student_answers_an_event_of_another_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var announcement = await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand(
            "Muestra de fin de año", null, AnnouncementCategory.Event, [scenario.Instructors.OtherClassGroup.Id]));

        using var response = await scenario.Student.PutAnnouncementReplyAsync(announcement.Id, isGoing: true);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
