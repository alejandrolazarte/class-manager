namespace ClassManager.Core.Abstractions.Persistence;

public sealed record BookedStudent(Guid StudentId, string StudentFullName, DateOnly? BirthDate, string ClientFullName);
