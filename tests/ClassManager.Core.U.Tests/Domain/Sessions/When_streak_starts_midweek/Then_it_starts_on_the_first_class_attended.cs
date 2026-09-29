using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_streak_starts_midweek;

public sealed class Then_it_starts_on_the_first_class_attended
{
    [Fact]
    public void Then_it_starts_on_the_first_class_attended_Run()
    {
        var thisMonday = new DateOnly(2026, 9, 21);
        var firstClass = thisMonday.AddDays(-5);
        var marks = new[]
        {
            new AttendanceMark(firstClass, AttendanceStatus.Present),
            new AttendanceMark(firstClass.AddDays(2), AttendanceStatus.Present),
        };

        var streak = AttendanceStreaks.Calculate(marks, TestData.Today);

        streak.Since.ShouldBe(firstClass);
    }
}
