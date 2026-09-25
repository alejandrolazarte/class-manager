namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record ClassGroupDetails(
    string? Name,
    Guid? InstructorId,
    IReadOnlyList<DayOfWeek>? Weekdays,
    string? StartTime,
    int? DurationMinutes,
    int? Capacity,
    string? Location);
