namespace ClassManager.Core.Domain.Sessions;

public static class AttendanceStreaks
{
    public const int RecentWeekCount = 12;
    public const int LookBackWeeks = 52;

    private const int DaysPerWeek = 7;

    public static DateOnly WeekStartOf(DateOnly date) =>
        date.AddDays(-(((int)date.DayOfWeek - (int)DayOfWeek.Monday + DaysPerWeek) % DaysPerWeek));

    public static DateOnly FirstDayToLoad(DateOnly today) => WeekStartOf(today).AddDays(-LookBackWeeks * DaysPerWeek);

    public static AttendanceStreak Calculate(IReadOnlyCollection<AttendanceMark> marks, DateOnly today)
    {
        var marksByWeek = marks.ToLookup(mark => WeekStartOf(mark.Date));
        var currentWeek = WeekStartOf(today);
        WeekAttendance AttendanceIn(DateOnly weekStart) => StatusOf(marksByWeek[weekStart]);

        var weeks = 0;
        DateOnly? since = null;
        for (var weekStart = currentWeek; weekStart >= currentWeek.AddDays(-LookBackWeeks * DaysPerWeek); weekStart = weekStart.AddDays(-DaysPerWeek))
        {
            var attendance = AttendanceIn(weekStart);
            if (attendance == WeekAttendance.Excused || (weekStart == currentWeek && attendance == WeekAttendance.NoClasses))
            {
                continue;
            }

            if (attendance != WeekAttendance.Attended)
            {
                break;
            }

            weeks++;
            since = marksByWeek[weekStart]
                .Where(mark => mark.Status == AttendanceStatus.Present)
                .Min(mark => mark.Date);
        }

        var recentWeeks = Enumerable.Range(0, RecentWeekCount)
            .Select(weeksAgo => currentWeek.AddDays(-(RecentWeekCount - 1 - weeksAgo) * DaysPerWeek))
            .Select(weekStart => new AttendanceWeek(weekStart, AttendanceIn(weekStart)))
            .ToList();

        return new AttendanceStreak(weeks, since, recentWeeks);
    }

    private static WeekAttendance StatusOf(IEnumerable<AttendanceMark> weekMarks)
    {
        var marks = weekMarks.ToList();
        if (marks.Any(mark => mark.Status == AttendanceStatus.Absent && !mark.IsExcused))
        {
            return WeekAttendance.Missed;
        }

        if (marks.Any(mark => mark.Status == AttendanceStatus.Present))
        {
            return WeekAttendance.Attended;
        }

        return marks.Count > 0 ? WeekAttendance.Excused : WeekAttendance.NoClasses;
    }
}
