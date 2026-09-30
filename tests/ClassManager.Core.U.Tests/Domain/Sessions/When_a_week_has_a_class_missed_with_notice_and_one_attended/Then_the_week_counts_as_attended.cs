using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_a_week_has_a_class_missed_with_notice_and_one_attended;

public sealed class Then_the_week_counts_as_attended
{
    [Fact]
    public void Then_the_week_counts_as_attended_Run()
    {
        var thisMonday = new DateOnly(2026, 9, 21);
        var marks = new[]
        {
            new AttendanceMark(thisMonday.AddDays(-7), AttendanceStatus.Absent, IsExcused: true),
            new AttendanceMark(thisMonday.AddDays(-5), AttendanceStatus.Present),
        };

        var streak = AttendanceStreaks.Calculate(marks, TestData.Today);

        streak.Weeks.ShouldBe(1);
    }
}
