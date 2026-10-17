using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.UseCases.Clients;

public static class AttendingContact
{
    private const string MustBeAdultMessage = "A client who attends class must be an adult; register their guardian as the client and them as a student.";

    public static bool NamesTheContact(string? contactFullName, string? studentFullName) =>
        string.Equals(contactFullName?.Trim(), studentFullName?.Trim(), StringComparison.OrdinalIgnoreCase);

    public static ResultError? MinorError(DateOnly? birthDate, DateOnly today, string fieldName) =>
        birthDate is { } date && !PersonAge.IsAdult(date, today)
            ? new ResultError(ClientErrorCodes.ContactMustBeAdult, MustBeAdultMessage, ErrorKind.Validation) { FieldName = fieldName }
            : null;
}
