using ClassManager.Core.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyAbsences.When_a_class_was_missed_with_notice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_streak_keeps_going(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_streak_keeps_going_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var studentId = scenario.Coaches.CoachStudentId;
        var today = CoachScenario.ClassDate;
        var weekly = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(scenario.Coaches.CoachInstructorId, [today.DayOfWeek], "08:00") with { Name = "Natación semanal" });
        await owner.EnrollAsync(weekly.Id, studentId, today.AddDays(-21));
        (await owner.PutAttendanceAsync(weekly.Id, today.AddDays(-21), studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();
        (await owner.PutAttendanceAsync(weekly.Id, today.AddDays(-14), studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();
        (await owner.PutAttendanceAsync(weekly.Id, today.AddDays(-7), studentId, AttendanceStatus.Absent)).EnsureSuccessStatusCode();
        await using (var context = fixture.CreateDbContext(scenario.Coaches.Business.Business.Id))
        {
            var missedSession = await context.ClassSessions.SingleAsync(session => session.ClassGroupId == weekly.Id && session.Date == today.AddDays(-7));
            context.AbsenceNotices.Add(AbsenceNotice.Create(missedSession.Id, studentId, keepsStreak: true, BusinessApiFactory.Now.AddDays(-8)));
            await context.SaveChangesAsync();
        }

        var home = await scenario.Family.GetFamilyHomeAsync();

        home!.Students.Single().Attendance.StreakWeeks.ShouldBe(2);
    }
}
