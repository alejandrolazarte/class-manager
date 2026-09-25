using ClassManager.Core.Abstractions.Persistence;

namespace ClassManager.Core.UseCases.Enrollments;

public sealed record RosterEntryResponse(
    Guid EnrollmentId,
    Guid StudentId,
    string StudentFullName,
    DateOnly? BirthDate,
    Guid ClientId,
    string ClientFullName,
    DateOnly StartDate,
    DateOnly? EndDate)
{
    public static RosterEntryResponse From(RosterEntry entry) =>
        new(entry.EnrollmentId, entry.StudentId, entry.StudentFullName, entry.BirthDate, entry.ClientId, entry.ClientFullName, entry.StartDate, entry.EndDate);
}
