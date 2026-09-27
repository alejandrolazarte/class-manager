using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.Abstractions.Persistence;

public sealed record ClientAttendedClass(Guid ClientId, AttendedClass AttendedClass);
