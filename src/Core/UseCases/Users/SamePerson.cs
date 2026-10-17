using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.UseCases.Users;

public static class SamePerson
{
    private const string SamePersonMessage = "Another person of the business already uses this email; if it is the same person, confirm it to join them.";

    public static async Task<Result<User?>> JoinOrAskAsync(
        IUserRepository userRepository,
        User person,
        PersonRole newRole,
        Guid? confirmedUserId,
        string fieldName,
        bool canJoin,
        CancellationToken cancellationToken)
    {
        if (person.Email is not { } email
            || await userRepository.FindKnownPersonByEmailAsync(email, person.Id, cancellationToken) is not { } knownPerson)
        {
            return Result.Success<User?>(null);
        }

        if (!canJoin || knownPerson.Roles.Contains(newRole))
        {
            return PeopleEmails.EmailTaken(fieldName);
        }

        if (confirmedUserId != knownPerson.UserId)
        {
            return Question(knownPerson, fieldName);
        }

        var samePerson = await userRepository.GetForUpdateAsync(knownPerson.UserId, cancellationToken);
        return samePerson is null ? PeopleEmails.EmailTaken(fieldName) : samePerson;
    }

    private static ResultError Question(KnownPerson knownPerson, string fieldName) =>
        new(UserErrorCodes.SamePerson, SamePersonMessage, ErrorKind.Conflict)
        {
            FieldName = fieldName,
            Details = new Dictionary<string, object?>
            {
                [UserErrorCodes.UserIdDetail] = knownPerson.UserId,
                [UserErrorCodes.FullNameDetail] = knownPerson.FullName,
                [UserErrorCodes.RolesDetail] = knownPerson.Roles.Select(role => role.ToString()).ToList(),
            },
        };
}
