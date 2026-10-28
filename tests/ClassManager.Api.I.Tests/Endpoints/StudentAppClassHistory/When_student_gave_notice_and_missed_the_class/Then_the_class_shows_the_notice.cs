using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.StudentApp;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassHistory.When_student_gave_notice_and_missed_the_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_class_shows_the_notice(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_class_shows_the_notice_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var studentId = scenario.Instructors.InstructorStudentId;
        var today = InstructorScenario.ClassDate;
        var weekly = await scenario.EnrollInWeeklyClassSinceAsync(today.AddDays(-7));
        (await scenario.Instructors.Business.HttpClient.PutAttendanceAsync(weekly.Id, today.AddDays(-7), studentId, AttendanceStatus.Absent)).EnsureSuccessStatusCode();
        await using (var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id))
        {
            var missedSession = await context.ClassSessions.SingleAsync(session => session.ClassGroupId == weekly.Id && session.Date == today.AddDays(-7));
            context.AbsenceNotices.Add(AbsenceNotice.Create(missedSession, studentId, keepsStreak: true, BusinessApiFactory.Now.AddDays(-8)));
            await context.SaveChangesAsync();
        }

        var history = await scenario.Student.GetClassHistoryAsync(studentId, ClassHistoryRequests.CurrentMonth);

        history!.Classes.Single().Status.ShouldBe(PastClassStatus.AbsenceNotified);
    }
}
