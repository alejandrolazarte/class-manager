using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppAbsences.When_a_notice_was_given_while_absences_broke_the_streak;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_streak_is_broken(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_streak_is_broken_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var studentId = scenario.Instructors.InstructorStudentId;
        var today = InstructorScenario.ClassDate;
        var weekly = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(scenario.Instructors.InstructorId, [today.DayOfWeek], "08:00") with { Name = "Natación semanal" });
        await owner.EnrollAsync(weekly.Id, studentId, today.AddDays(-21));
        (await owner.PutAttendanceAsync(weekly.Id, today.AddDays(-21), studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();
        (await owner.PutAttendanceAsync(weekly.Id, today.AddDays(-14), studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();
        await using (var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id))
        {
            var missedSession = ClassSession.Create(weekly.Id, today.AddDays(-7), BusinessApiFactory.Now.AddDays(-8));
            context.ClassSessions.Add(missedSession);
            context.AbsenceNotices.Add(AbsenceNotice.Create(missedSession.Id, studentId, keepsStreak: false, BusinessApiFactory.Now.AddDays(-8)));
            await context.SaveChangesAsync();
        }

        var home = await scenario.Student.GetStudentAppHomeAsync();

        home!.Students.Single().Attendance.StreakWeeks.ShouldBe(0);
    }
}
