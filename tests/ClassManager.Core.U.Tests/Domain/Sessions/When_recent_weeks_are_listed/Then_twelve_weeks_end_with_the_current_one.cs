using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Sessions.When_recent_weeks_are_listed;

public sealed class Then_twelve_weeks_end_with_the_current_one
{
    [Fact]
    public void Then_twelve_weeks_end_with_the_current_one_Run()
    {
        var thisMonday = new DateOnly(2026, 9, 21);
        var marks = new[]
        {
            new AttendanceMark(thisMonday.AddDays(-7), AttendanceStatus.Absent),
            new AttendanceMark(thisMonday.AddDays(2), AttendanceStatus.Present),
        };

        var streak = AttendanceStreaks.Calculate(marks, TestData.Today);

        streak.RecentWeeks.Count.ShouldBe(AttendanceStreaks.RecentWeekCount);
        streak.RecentWeeks[^1].ShouldBe(new AttendanceWeek(thisMonday, WeekAttendance.Attended));
        streak.RecentWeeks[^2].ShouldBe(new AttendanceWeek(thisMonday.AddDays(-7), WeekAttendance.Missed));
        streak.RecentWeeks[0].ShouldBe(new AttendanceWeek(thisMonday.AddDays(-77), WeekAttendance.NoClasses));
    }
}
