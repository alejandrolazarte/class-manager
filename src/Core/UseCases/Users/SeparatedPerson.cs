using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.UseCases.Users;

public sealed record SeparatePersonRequest(string? FullName, string? Email);

public sealed record SeparatedPersonResponse(Guid UserId, string FullName);

public static class SeparatedPerson
{
    private const string NothingToSeparateMessage = "This person has no other role in the business to separate from.";

    public static ResultError NothingToSeparate() =>
        new(UserErrorCodes.NothingToSeparate, NothingToSeparateMessage, ErrorKind.Conflict);

    public static async Task<Result<User>> CreateAsync(
        IUserRepository userRepository,
        string? fullName,
        string? email,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var person = User.Create(fullName, email, now);
        if (person.IsFailure)
        {
            return person.Error!;
        }

        if (await PeopleEmails.TakenErrorAsync(userRepository, [(person.Value!, nameof(SeparatePersonRequest.Email))], cancellationToken) is { } emailTaken)
        {
            return emailTaken;
        }

        userRepository.Add(person.Value!);
        return person.Value!;
    }
}
