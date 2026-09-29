using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_student_missed_a_class_two_weeks_ago;

public sealed class Then_the_streak_starts_after_that_week
{
    [Fact]
    public void Then_the_streak_starts_after_that_week_Run()
    {
        var thisMonday = new DateOnly(2026, 9, 21);
        var marks = new[]
        {
            new AttendanceMark(thisMonday.AddDays(-21), AttendanceStatus.Present),
            new AttendanceMark(thisMonday.AddDays(-14), AttendanceStatus.Present),
            new AttendanceMark(thisMonday.AddDays(-12), AttendanceStatus.Absent),
            new AttendanceMark(thisMonday.AddDays(-7), AttendanceStatus.Present),
            new AttendanceMark(thisMonday, AttendanceStatus.Present),
        };

        var streak = AttendanceStreaks.Calculate(marks, TestData.Today);

        streak.Weeks.ShouldBe(2);
        streak.Since.ShouldBe(thisMonday.AddDays(-7));
    }
}
