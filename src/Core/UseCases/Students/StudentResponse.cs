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
    public static StudentResponse From(Student student, StudentAppAccessResponse appAccess) =>
        new(student.Id, student.ClientId, student.User.FullName, student.User.BirthDate, student.Notes, student.User.Email, student.CreatedAt, appAccess);

    public static StudentResponse FromWritten(Student student) =>
        new(student.Id, student.ClientId, student.FullName, student.BirthDate, student.Notes, student.Email, student.CreatedAt, StudentAppAccessResponse.NotInvited);
}
