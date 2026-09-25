using ClassManager.Core.Abstractions.Persistence;

namespace ClassManager.Core.UseCases.Students;

public sealed record StudentSummaryResponse(
    Guid Id,
    string FullName,
    DateOnly? BirthDate,
    string? Notes,
    Guid ClientId,
    string ClientFullName,
    string ClientPhoneNumber)
{
    public static StudentSummaryResponse From(StudentSummary student) =>
        new(student.Id, student.FullName, student.BirthDate, student.Notes, student.ClientId, student.ClientFullName, student.ClientPhoneNumber);
}
