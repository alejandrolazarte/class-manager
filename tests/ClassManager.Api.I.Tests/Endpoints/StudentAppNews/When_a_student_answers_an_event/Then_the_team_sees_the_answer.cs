using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_a_student_answers_an_event;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_the_answer(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_sees_the_answer_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var announcement = await owner.PublishAnnouncementAsync(
            new CreateAnnouncementCommand("Muestra de fin de año", null, AnnouncementCategory.Event));

        (await scenario.Student.PutAnnouncementReplyAsync(announcement.Id, isGoing: true)).StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var recipient = (await owner.GetAnnouncementRecipientsAsync(announcement.Id))!.Single();
        recipient.IsGoing.ShouldBe(true);
        recipient.SeenAt.ShouldNotBeNull();
    }
}
