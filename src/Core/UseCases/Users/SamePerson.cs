using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.UseCases.Users;

public static class SamePerson
{
    public const string UserIdDetail = "samePersonUserId";
    public const string FullNameDetail = "samePersonFullName";
    public const string FieldDetail = "samePersonField";

    private const string QuestionMessage =
        "Another person of the business already uses this email. If it is the same person, send their id as SamePersonUserId.";

    public static async Task<Result<User?>> FindAsync(
        IUserRepository userRepository,
        User person,
        string? email,
        long? samePersonUserId,
        Func<PersonRoles, bool> canTakeTheRole,
        string fieldName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Success<User?>(null);
        }

        var otherPerson = await userRepository.FindByEmailForUpdateAsync(email.Trim(), person.Id, cancellationToken);
        if (otherPerson is null)
        {
            return Result.Success<User?>(null);
        }

        if (!canTakeTheRole(await userRepository.GetRolesAsync(otherPerson.Id, cancellationToken)))
        {
            return PeopleEmails.EmailTaken(fieldName);
        }

        return otherPerson.Id == samePersonUserId ? otherPerson : Question(otherPerson, fieldName);
    }

    public static async Task RemoveIfUnusedAsync(
        IUserRepository userRepository,
        User previousPerson,
        Func<PersonRoles, bool> isStillUsed,
        CancellationToken cancellationToken)
    {
        if (!isStillUsed(await userRepository.GetRolesAsync(previousPerson.Id, cancellationToken)))
        {
            userRepository.Remove(previousPerson);
        }
    }

    private static ResultError Question(User otherPerson, string fieldName) =>
        new(UserErrorCodes.SamePerson, QuestionMessage, ErrorKind.Conflict)
        {
            FieldName = fieldName,
            Details = new Dictionary<string, object?>
            {
                [UserIdDetail] = otherPerson.Id,
                [FullNameDetail] = otherPerson.FullName,
                [FieldDetail] = fieldName,
            },
        };
}
