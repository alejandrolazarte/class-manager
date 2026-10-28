using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public sealed record StudentClassAttendance(long ClassGroupId, DateOnly Date, AttendanceStatus Status);
