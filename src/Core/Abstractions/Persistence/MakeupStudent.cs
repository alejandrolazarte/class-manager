namespace ClassManager.Core.Abstractions.Persistence;

public sealed record MakeupStudent(Guid StudentId, string StudentFullName, DateOnly? BirthDate, string ClientFullName);
