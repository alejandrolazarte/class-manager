using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_a_student_turned_off_news_pushes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_an_announcement_sends_no_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_an_announcement_sends_no_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();
        await scenario.Student.TurnOffAsync(NotificationCategory.News, NotificationChannel.Push);
        var owner = scenario.Instructors.Business.HttpClient;

        await owner.PublishAnnouncementAsync(new CreateAnnouncementCommand("Cerramos el lunes", null));
        (await owner.PutSessionCancellationAsync(scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate, "Feriado")).EnsureSuccessStatusCode();

        (await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint)).Title.ShouldBe($"Se suspende {InstructorScenario.InstructorClassGroupName}");
        fixture.ApiFactory.PushSender.SentTo(endpoint).ShouldNotContain(push => push.Title == "Cerramos el lunes");
    }
}
