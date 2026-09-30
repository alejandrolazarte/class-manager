using ClassManager.Core.Domain.Achievements;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.U.Tests.Domain.Achievements.When_a_month_was_attended_without_misses;

public sealed class Then_perfect_month_is_earned
{
    [Fact]
    public void Then_perfect_month_is_earned_Run()
    {
        var august = new DateOnly(2026, 8, 3);
        var marks = Enumerable.Range(0, 4).Select(week => new AttendanceMark(august.AddDays(week * 7), AttendanceStatus.Present)).ToList();

        var medals = Medals.Earned(new MedalProgress(AttendedClasses: 4, Level: 1, BestStreakWeeks: 4), marks, TestData.Today);

        medals.ShouldBe([Medal.FirstClass, Medal.FourWeekStreak, Medal.PerfectMonth]);
    }
}
