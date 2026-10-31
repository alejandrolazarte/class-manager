using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Fees;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Core.UseCases.Clients;

public sealed record ClientDetailsResponse(
    long Id,
    string FullName,
    string PhoneNumber,
    string? Email,
    string? Notes,
    DateTimeOffset CreatedAt,
    FamilyPlansResponse Plans,
    IReadOnlyList<StudentResponse> Students,
    StudentAppAccessResponse AppAccess,
    bool IsOnTheTeam = false,
    string? PhotoUrl = null)
{
    public static ClientDetailsResponse From(
        Client client,
        IReadOnlyCollection<Student> students,
        FamilyPlansResponse plans,
        StudentAppAccessResponse appAccess,
        IReadOnlyDictionary<long, StudentAppAccessResponse> studentAppAccesses) =>
        new(
            client.Id,
            client.User.FullName,
            client.User.PhoneNumber!.Value,
            client.User.Email,
            client.Notes,
            client.CreatedAt,
            plans,
            [.. students
                .OrderBy(student => student.User.FullName, StringComparer.CurrentCultureIgnoreCase)
                .Select(student => StudentResponse.From(student, studentAppAccesses.GetValueOrDefault(student.Id, StudentAppAccessResponse.NotInvited)))],
            appAccess);

    public static ClientDetailsResponse Registered(Client client, IReadOnlyCollection<Student> students, FamilyPlansResponse plans) =>
        new(
            client.Id,
            client.User.FullName,
            client.User.PhoneNumber!.Value,
            client.User.Email,
            client.Notes,
            client.CreatedAt,
            plans,
            [.. students
                .OrderBy(student => student.User.FullName, StringComparer.CurrentCultureIgnoreCase)
                .Select(StudentResponse.NotInvited)],
            StudentAppAccessResponse.NotInvited);
}
