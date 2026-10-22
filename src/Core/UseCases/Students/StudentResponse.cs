using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Core.UseCases.Students;

public sealed record StudentResponse(
    long Id,
    long ClientId,
    string FullName,
    DateOnly? BirthDate,
    string? Notes,
    string? Email,
    DateTimeOffset CreatedAt,
    StudentAppAccessResponse AppAccess,
    bool IsOnTheTeam = false)
{
    public static StudentResponse From(Student student, StudentAppAccessResponse appAccess) =>
        new(student.Id, student.ClientId, student.User.FullName, student.User.BirthDate, student.Notes, student.User.Email, student.CreatedAt, appAccess);

    public static StudentResponse NotInvited(Student student) => From(student, StudentAppAccessResponse.NotInvited);
}
