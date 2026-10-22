namespace ClassManager.Core.Abstractions.Persistence;

public sealed record RosterEntry(
    long EnrollmentId,
    long StudentId,
    string StudentFullName,
    DateOnly? BirthDate,
    long ClientId,
    string ClientFullName,
    DateOnly StartDate,
    DateOnly? EndDate);
