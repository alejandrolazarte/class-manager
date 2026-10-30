using ClassManager.Core.Abstractions.Persistence;

namespace ClassManager.Core.UseCases.Students;

public sealed record StudentSummaryResponse(
    long Id,
    string FullName,
    DateOnly? BirthDate,
    string? Notes,
    long ClientId,
    string ClientFullName,
    string ClientPhoneNumber,
    string? PhotoUrl = null)
{
    public static StudentSummaryResponse From(StudentSummary student, string? photoUrl = null) =>
        new(student.Id, student.FullName, student.BirthDate, student.Notes, student.ClientId, student.ClientFullName, student.ClientPhoneNumber, photoUrl);
}
