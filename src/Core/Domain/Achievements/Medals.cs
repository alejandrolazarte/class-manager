using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Domain.Achievements;

public static class Medals
{
    public const int TenClasses = 10;
    public const int HundredClasses = 100;
    public const int StreakWeeks = 4;
    public const int PerfectMonthClasses = 4;

    public static IReadOnlyList<Medal> Earned(MedalProgress progress, IReadOnlyCollection<AttendanceMark> marks, DateOnly today)
    {
        var earned = new List<Medal>();
        if (progress.AttendedClasses >= 1)
        {
            earned.Add(Medal.FirstClass);
        }

        if (progress.AttendedClasses >= TenClasses)
        {
            earned.Add(Medal.TenClasses);
        }

        if (progress.BestStreakWeeks >= StreakWeeks)
        {
            earned.Add(Medal.FourWeekStreak);
        }

        if (progress.Level > 1)
        {
            earned.Add(Medal.LeveledUp);
        }

        if (HasPerfectMonth(marks, today))
        {
            earned.Add(Medal.PerfectMonth);
        }

        if (progress.AttendedClasses >= HundredClasses)
        {
            earned.Add(Medal.HundredClasses);
        }

        return earned;
    }

    private static bool HasPerfectMonth(IReadOnlyCollection<AttendanceMark> marks, DateOnly today)
    {
        var currentMonth = new DateOnly(today.Year, today.Month, 1);
        return marks
            .Where(mark => mark.Date < currentMonth)
            .GroupBy(mark => new DateOnly(mark.Date.Year, mark.Date.Month, 1))
            .Any(month =>
                month.Count(mark => mark.Status == AttendanceStatus.Present) >= PerfectMonthClasses
                && !month.Any(mark => mark.Status == AttendanceStatus.Absent && !mark.IsExcused));
    }
}
