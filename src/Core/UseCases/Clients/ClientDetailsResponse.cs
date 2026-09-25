using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Core.UseCases.Clients;

public sealed record ClientDetailsResponse(
    Guid Id,
    string FullName,
    string PhoneNumber,
    string? Email,
    string? Notes,
    DateTimeOffset CreatedAt,
    IReadOnlyList<StudentResponse> Students)
{
    public static ClientDetailsResponse From(Client client, IEnumerable<Student> students) =>
        new(
            client.Id,
            client.FullName,
            client.PhoneNumber.Value,
            client.Email,
            client.Notes,
            client.CreatedAt,
            [.. students.OrderBy(student => student.FullName, StringComparer.CurrentCultureIgnoreCase).Select(StudentResponse.From)]);
}
