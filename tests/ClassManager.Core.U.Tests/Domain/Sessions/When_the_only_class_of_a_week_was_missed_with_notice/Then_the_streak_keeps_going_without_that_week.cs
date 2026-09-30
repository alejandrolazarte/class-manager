using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_the_only_class_of_a_week_was_missed_with_notice;

public sealed class Then_the_streak_keeps_going_without_that_week
{
    [Fact]
    public void Then_the_streak_keeps_going_without_that_week_Run()
    {
        var thisMonday = new DateOnly(2026, 9, 21);
        var marks = new[]
        {
            new AttendanceMark(thisMonday.AddDays(-14), AttendanceStatus.Present),
            new AttendanceMark(thisMonday.AddDays(-7), AttendanceStatus.Absent, IsExcused: true),
            new AttendanceMark(thisMonday, AttendanceStatus.Present),
        };

        var streak = AttendanceStreaks.Calculate(marks, TestData.Today);

        streak.Weeks.ShouldBe(2);
        streak.Since.ShouldBe(thisMonday.AddDays(-14));
        streak.RecentWeeks[^2].Attendance.ShouldBe(WeekAttendance.Excused);
    }
}
