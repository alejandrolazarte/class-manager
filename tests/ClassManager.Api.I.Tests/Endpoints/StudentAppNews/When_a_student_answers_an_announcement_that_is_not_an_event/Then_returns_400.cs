using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_a_student_answers_an_announcement_that_is_not_an_event;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var announcement = await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync();

        using var response = await scenario.Student.PutAnnouncementReplyAsync(announcement.Id, isGoing: true);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AnnouncementReply.NotAnEventCode);
    }
}
