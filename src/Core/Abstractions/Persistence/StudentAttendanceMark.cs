using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.Abstractions.Persistence;

public sealed record StudentAttendanceMark(long StudentId, AttendanceMark Mark);
