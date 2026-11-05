using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_a_student_turned_off_class_change_pushes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_cancelled_class_sends_no_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_cancelled_class_sends_no_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();
        await scenario.Student.TurnOffAsync(NotificationCategory.ClassChanges, NotificationChannel.Push);

        (await scenario.Instructors.Business.HttpClient.PutSessionCancellationAsync(
            scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate.AddDays(7), "Feriado")).EnsureSuccessStatusCode();
        (await scenario.Instructors.Instructor.PutFeedbackAsync(
            scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.Instructors.InstructorStudentId, "Muy buena patada")).EnsureSuccessStatusCode();

        (await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint)).Body.ShouldBe("Muy buena patada");
        fixture.ApiFactory.PushSender.SentTo(endpoint).ShouldNotContain(push => push.Title == $"Se suspende {InstructorScenario.InstructorClassGroupName}");
    }
}
