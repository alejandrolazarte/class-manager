using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_comes_to_class_every_week;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_student_home_shows_the_streak(ApiFixture fixture)
{
    private const int WeeksAttended = 4;

    [Fact]
    public async Task Then_student_home_shows_the_streak_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var today = CoachScenario.ClassDate;
        var firstClass = today.AddDays(-7 * (WeeksAttended - 1));
        var weeklyClassGroup = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(scenario.Coaches.CoachInstructorId, [today.DayOfWeek], "10:00") with { Name = "Natación semanal" });
        await owner.EnrollAsync(weeklyClassGroup.Id, scenario.Coaches.CoachStudentId, firstClass);
        for (var classDate = firstClass; classDate <= today; classDate = classDate.AddDays(7))
        {
            using var response = await owner.PutAttendanceAsync(
                weeklyClassGroup.Id, classDate, scenario.Coaches.CoachStudentId, AttendanceStatus.Present);
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        var home = await scenario.Student.GetStudentAppHomeAsync();

        var attendance = home!.Students.Single().Attendance;
        attendance.StreakWeeks.ShouldBe(WeeksAttended);
        attendance.StreakSince.ShouldBe(firstClass);
        attendance.AttendedClasses.ShouldBe(WeeksAttended);
        attendance.RecentWeeks[^1].Attendance.ShouldBe(WeekAttendance.Attended);
    }
}
