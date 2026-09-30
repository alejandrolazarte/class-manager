namespace ClassManager.Core.Domain.Sessions;

public sealed record AttendanceMark(DateOnly Date, AttendanceStatus Status, bool IsExcused = false);
