using ClassManager.Core.Audit;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record ClassGroupDetails(
    [property: AuditPlain] string? Name,
    Guid? InstructorId,
    IReadOnlyList<DayOfWeek>? Weekdays,
    [property: AuditPlain] string? StartTime,
    int? DurationMinutes,
    int? Capacity,
    [property: AuditPlain] string? Location,
    [property: AuditPlain] string? MaterialUrl = null);
