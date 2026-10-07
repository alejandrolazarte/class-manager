using ClassManager.Core.Common;
using ClassManager.Core.Domain.Students;

namespace ClassManager.Core.UseCases.Students;

internal static class FamilyEmails
{
    private const string EmailOfAnotherPersonMessage = "This email belongs to another person of the family; each person needs their own email.";

    public static bool IsTakenByAnotherPerson(string? email, string? clientEmail, IEnumerable<Student> otherStudents) =>
        !string.IsNullOrWhiteSpace(email)
        && (string.Equals(email.Trim(), clientEmail, StringComparison.OrdinalIgnoreCase) || otherStudents.Any(student => student.HasEmail(email)));

    public static ResultError EmailOfAnotherPerson(string fieldName) =>
        new(StudentErrorCodes.EmailOfAnotherPerson, EmailOfAnotherPersonMessage, ErrorKind.Validation) { FieldName = fieldName };
}
