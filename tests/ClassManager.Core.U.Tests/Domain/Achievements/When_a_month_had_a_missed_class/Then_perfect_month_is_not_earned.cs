using ClassManager.Core.Domain.Achievements;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Achievements.When_a_month_had_a_missed_class;

public sealed class Then_perfect_month_is_not_earned
{
    [Fact]
    public void Then_perfect_month_is_not_earned_Run()
    {
        var august = new DateOnly(2026, 8, 3);
        var marks = Enumerable.Range(0, 4).Select(week => new AttendanceMark(august.AddDays(week * 7), AttendanceStatus.Present))
            .Append(new AttendanceMark(august.AddDays(2), AttendanceStatus.Absent))
            .ToList();

        var medals = Medals.Earned(new MedalProgress(AttendedClasses: 4, Level: 2, BestStreakWeeks: 0), marks, TestData.Today);

        medals.ShouldBe([Medal.FirstClass, Medal.LeveledUp]);
    }
}
