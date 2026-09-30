using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_an_older_streak_was_longer;

public sealed class Then_best_weeks_keeps_it
{
    [Fact]
    public void Then_best_weeks_keeps_it_Run()
    {
        var thisMonday = new DateOnly(2026, 9, 21);
        var marks = new[]
        {
            new AttendanceMark(thisMonday.AddDays(-42), AttendanceStatus.Present),
            new AttendanceMark(thisMonday.AddDays(-35), AttendanceStatus.Present),
            new AttendanceMark(thisMonday.AddDays(-28), AttendanceStatus.Present),
            new AttendanceMark(thisMonday.AddDays(-21), AttendanceStatus.Absent),
            new AttendanceMark(thisMonday.AddDays(-14), AttendanceStatus.Present),
        };

        var streak = AttendanceStreaks.Calculate(marks, TestData.Today);

        streak.BestWeeks.ShouldBe(3);
    }
}
