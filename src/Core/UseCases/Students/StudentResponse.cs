using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.UseCases.Students;

public sealed record StudentResponse(
    Guid Id,
    Guid ClientId,
    string FullName,
    DateOnly? BirthDate,
    string? Notes,
    string? Email,
    DateTimeOffset CreatedAt,
    StudentAppAccessResponse AppAccess)
{
    public static StudentResponse From(Student student) => From(student, StudentAppAccessResponse.NotInvited);

    public static StudentResponse From(Student student, StudentAppAccessResponse appAccess) =>
        new(student.Id, student.ClientId, student.FullName, student.BirthDate, student.Notes, student.Email, student.CreatedAt, appAccess);
}
