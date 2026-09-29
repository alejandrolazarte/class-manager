using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_current_week_has_no_classes_yet;

public sealed class Then_the_streak_keeps_the_previous_weeks
{
    [Fact]
    public void Then_the_streak_keeps_the_previous_weeks_Run()
    {
        var thisMonday = new DateOnly(2026, 9, 21);
        var marks = new[]
        {
            new AttendanceMark(thisMonday.AddDays(-14), AttendanceStatus.Present),
            new AttendanceMark(thisMonday.AddDays(-7), AttendanceStatus.Present),
        };

        var streak = AttendanceStreaks.Calculate(marks, TestData.Today);

        streak.Weeks.ShouldBe(2);
        streak.Since.ShouldBe(thisMonday.AddDays(-14));
    }
}
