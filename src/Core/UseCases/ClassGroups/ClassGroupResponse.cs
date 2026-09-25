using System.Globalization;
using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record ClassGroupResponse(
    Guid Id,
    string Name,
    Guid InstructorId,
    string InstructorFullName,
    IReadOnlyList<DayOfWeek> Weekdays,
    string StartTime,
    string EndTime,
    int DurationMinutes,
    int Capacity,
    string? Location,
    bool IsActive)
{
    public static ClassGroupResponse From(ClassGroup classGroup, string instructorFullName)
    {
        var schedule = classGroup.Schedule;
        return new(
            classGroup.Id,
            classGroup.Name,
            classGroup.InstructorId,
            instructorFullName,
            schedule.Days,
            schedule.StartTime.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture),
            schedule.EndTime.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture),
            schedule.DurationMinutes,
            classGroup.Capacity,
            classGroup.Location,
            classGroup.IsActive);
    }
}
