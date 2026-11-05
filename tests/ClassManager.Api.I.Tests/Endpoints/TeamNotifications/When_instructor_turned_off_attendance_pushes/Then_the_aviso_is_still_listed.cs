using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_instructor_turned_off_attendance_pushes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_aviso_is_still_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_aviso_is_still_listed_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.Instructors.Instructor.TurnOffAsync(NotificationCategory.Attendance, NotificationChannel.Push);

        (await scenario.Student.PutAbsenceAsync(
            scenario.Instructors.InstructorStudentId, scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate.AddDays(7))).EnsureSuccessStatusCode();

        var notifications = await scenario.Instructors.Instructor.GetTeamNotificationsAsync();
        notifications!.Items.ShouldContain(notification => notification.Title == "Tomás avisó que no viene");
    }
}
