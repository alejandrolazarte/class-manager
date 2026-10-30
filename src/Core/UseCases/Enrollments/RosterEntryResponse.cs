using ClassManager.Core.Abstractions.Persistence;

namespace ClassManager.Core.UseCases.Enrollments;

public sealed record RosterEntryResponse(
    long EnrollmentId,
    long StudentId,
    string StudentFullName,
    DateOnly? BirthDate,
    long ClientId,
    string ClientFullName,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? StudentPhotoUrl = null)
{
    public static RosterEntryResponse From(RosterEntry entry, string? studentPhotoUrl = null) =>
        new(entry.EnrollmentId, entry.StudentId, entry.StudentFullName, entry.BirthDate, entry.ClientId, entry.ClientFullName, entry.StartDate, entry.EndDate, studentPhotoUrl);
}
