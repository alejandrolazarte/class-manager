using ClassManager.Core.Common;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.UseCases.Students;

internal static class FamilyEmails
{
    private const string EmailOfAnotherPersonMessage = "This email belongs to another person of the family; each person needs their own email.";

    public static bool IsTakenByAnotherPerson(User person, IEnumerable<User> family) =>
        person.Email is not null && family.Any(other => other != person && other.HasEmail(person.Email));

    public static ResultError EmailOfAnotherPerson(string fieldName) =>
        new(StudentErrorCodes.EmailOfAnotherPerson, EmailOfAnotherPersonMessage, ErrorKind.Validation) { FieldName = fieldName };
}
