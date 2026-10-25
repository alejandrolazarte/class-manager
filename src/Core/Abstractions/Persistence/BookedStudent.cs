namespace ClassManager.Core.Abstractions.Persistence;

public sealed record BookedStudent(long StudentId, string StudentFullName, DateOnly? BirthDate, long ClientId, string ClientFullName);
