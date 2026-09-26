using System.Globalization;
using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.ClassGroups;

public sealed class ClassSchedule
{
    public const string TimeFormat = "HH:mm";
    public const int MinimumDurationMinutes = 15;
    public const int MaximumDurationMinutes = 240;
    public const int DurationStepMinutes = 5;

    private const int MinutesPerDay = 24 * 60;
    private const string WeekdaysRequiredMessage = "Choose at least one weekday.";
    private const string StartTimeFormatMessage = "Start time must use the HH:mm format.";
    private const string DurationRangeMessage = "Duration must be between 15 and 240 minutes, in steps of 5, and end by midnight.";

    private static readonly DayOfWeek[] WeekOrder =
    [
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
        DayOfWeek.Sunday,
    ];

    private ClassSchedule(ClassWeekdays weekdays, TimeOnly startTime, int durationMinutes)
    {
        Weekdays = weekdays;
        StartTime = startTime;
        DurationMinutes = durationMinutes;
    }

    public ClassWeekdays Weekdays { get; }
    public TimeOnly StartTime { get; }
    public int DurationMinutes { get; }
    public TimeOnly EndTime => StartTime.AddMinutes(DurationMinutes);
    public IReadOnlyList<DayOfWeek> Days => [.. WeekOrder.Where(MeetsOn)];

    private int StartMinute => (StartTime.Hour * 60) + StartTime.Minute;
    private int EndMinute => StartMinute + DurationMinutes;

    public static Result<ClassSchedule> Create(IEnumerable<DayOfWeek>? days, string? startTime, int? durationMinutes)
    {
        var weekdays = (days ?? []).Aggregate(ClassWeekdays.None, (combined, day) => combined | ToFlag(day));
        if (weekdays == ClassWeekdays.None)
        {
            return Result.Validation<ClassSchedule>(WeekdaysRequiredMessage, fieldName: nameof(Weekdays));
        }

        if (!TimeOnly.TryParseExact(startTime?.Trim(), TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedStartTime))
        {
            return Result.Validation<ClassSchedule>(StartTimeFormatMessage, fieldName: nameof(StartTime));
        }

        var schedule = new ClassSchedule(weekdays, parsedStartTime, durationMinutes ?? 0);
        if (schedule.DurationMinutes is < MinimumDurationMinutes or > MaximumDurationMinutes
            || schedule.DurationMinutes % DurationStepMinutes != 0
            || schedule.EndMinute > MinutesPerDay)
        {
            return Result.Validation<ClassSchedule>(DurationRangeMessage, fieldName: nameof(DurationMinutes));
        }

        return schedule;
    }

    public static ClassSchedule FromStored(ClassWeekdays weekdays, TimeOnly startTime, int durationMinutes) =>
        new(weekdays, startTime, durationMinutes);

    public static ClassSchedule ForDay(DayOfWeek day, TimeOnly startTime, int durationMinutes) =>
        new(ToFlag(day), startTime, durationMinutes);

    public bool MeetsOn(DayOfWeek day) => (Weekdays & ToFlag(day)) != ClassWeekdays.None;

    public bool OverlapsWith(ClassSchedule other) =>
        (Weekdays & other.Weekdays) != ClassWeekdays.None
        && StartMinute < other.EndMinute
        && other.StartMinute < EndMinute;

    private static ClassWeekdays ToFlag(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => ClassWeekdays.Monday,
        DayOfWeek.Tuesday => ClassWeekdays.Tuesday,
        DayOfWeek.Wednesday => ClassWeekdays.Wednesday,
        DayOfWeek.Thursday => ClassWeekdays.Thursday,
        DayOfWeek.Friday => ClassWeekdays.Friday,
        DayOfWeek.Saturday => ClassWeekdays.Saturday,
        DayOfWeek.Sunday => ClassWeekdays.Sunday,
        _ => ClassWeekdays.None,
    };
}
