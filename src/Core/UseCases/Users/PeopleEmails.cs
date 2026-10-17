using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.UseCases.Users;

public static class PeopleEmails
{
    private const string EmailTakenMessage = "Another person of the business already uses this email; each person needs their own.";

    public static async Task<ResultError?> TakenErrorAsync(
        IUserRepository userRepository,
        IEnumerable<(User Person, string FieldName)> people,
        CancellationToken cancellationToken)
    {
        foreach (var (person, fieldName) in people)
        {
            if (person.Email is { } email && await userRepository.IsEmailTakenAsync(email, person.Id, cancellationToken))
            {
                return EmailTaken(fieldName);
            }
        }

        return null;
    }

    public static ResultError EmailTaken(string fieldName) =>
        new(UserErrorCodes.EmailTaken, EmailTakenMessage, ErrorKind.Validation) { FieldName = fieldName };
}
