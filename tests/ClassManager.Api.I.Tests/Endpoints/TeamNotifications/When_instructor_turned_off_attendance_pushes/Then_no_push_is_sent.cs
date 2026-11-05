using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_instructor_turned_off_attendance_pushes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_push_is_sent(ApiFixture fixture)
{
    private const string AbsencePushText = "que no viene";

    [Fact]
    public async Task Then_no_push_is_sent_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.Instructors.Instructor.SubscribeToTeamPushAsync();
        await scenario.Instructors.Instructor.TurnOffAsync(NotificationCategory.Attendance, NotificationChannel.Push);

        (await scenario.Student.PutAbsenceAsync(
            scenario.Instructors.InstructorStudentId, scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate.AddDays(7))).EnsureSuccessStatusCode();

        await fixture.WaitUntilNoTaskToAsync(AbsencePushText);
        fixture.ApiFactory.PushSender.SentTo(endpoint).ShouldBeEmpty();
    }
}
