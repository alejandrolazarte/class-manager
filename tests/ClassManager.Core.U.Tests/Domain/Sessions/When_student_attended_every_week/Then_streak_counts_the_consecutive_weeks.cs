using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_student_attended_every_week;

public sealed class Then_streak_counts_the_consecutive_weeks
{
    [Fact]
    public void Then_streak_counts_the_consecutive_weeks_Run()
    {
        var thisMonday = new DateOnly(2026, 9, 21);
        var marks = new[]
        {
            new AttendanceMark(thisMonday.AddDays(-21), AttendanceStatus.Present),
            new AttendanceMark(thisMonday.AddDays(-14), AttendanceStatus.Present),
            new AttendanceMark(thisMonday.AddDays(-12), AttendanceStatus.Present),
            new AttendanceMark(thisMonday.AddDays(-7), AttendanceStatus.Present),
            new AttendanceMark(thisMonday.AddDays(1), AttendanceStatus.Present),
        };

        var streak = AttendanceStreaks.Calculate(marks, TestData.Today);

        streak.Weeks.ShouldBe(4);
        streak.Since.ShouldBe(thisMonday.AddDays(-21));
    }
}
