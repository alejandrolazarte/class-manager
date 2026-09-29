using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public sealed record StudentAttendanceMark(Guid StudentId, AttendanceMark Mark);
