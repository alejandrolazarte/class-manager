using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.Students;

public sealed record StudentResponse(
    Guid Id,
    Guid ClientId,
    string FullName,
    DateOnly? BirthDate,
    string? Notes,
    DateTimeOffset CreatedAt)
{
    public static StudentResponse From(Student student) =>
        new(student.Id, student.ClientId, student.FullName, student.BirthDate, student.Notes, student.CreatedAt);
}
