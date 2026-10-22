using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.PrivateLessons;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.PrivateLessons.When_PrivateLesson_with_attendance_is_cancelled;

public sealed class Then_returns_has_attendance_conflict
{
    [Fact]
    public void Then_returns_has_attendance_conflict_Run()
    {
        var studentId = TestIds.Next();
        var schedule = ClassSchedule.Create([TestData.Today.DayOfWeek], "18:00", 45).Value!;
        var lesson = PrivateLesson.Create(TestIds.Next(), TestData.Today, schedule, [studentId], null, null, null, TestData.Now).Value!.WithId();
        lesson.Mark(studentId, AttendanceStatus.Present);

        var cancel = lesson.Cancel("Feriado");

        cancel.Error!.Code.ShouldBe(SessionErrorCodes.HasAttendance);
    }
}
