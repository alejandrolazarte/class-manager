namespace ClassManager.Core.Abstractions.Persistence;

public sealed record RosterEntry(
    Guid EnrollmentId,
    Guid StudentId,
    string StudentFullName,
    DateOnly? BirthDate,
    Guid ClientId,
    string ClientFullName,
    DateOnly StartDate,
    DateOnly? EndDate);
