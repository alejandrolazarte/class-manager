using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.UseCases.Users;

public static class AccountUsers
{
    private const string AccountNotFoundMessage = "The account does not exist.";

    public static async Task<User> ResolveAsync(
        IUserRepository userRepository,
        IIdentityService identityService,
        User? person,
        Guid accountId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (person?.AccountId == accountId)
        {
            return person;
        }

        var userOfTheAccount = await userRepository.FindByAccountForUpdateAsync(accountId, cancellationToken);
        if (userOfTheAccount is not null)
        {
            return userOfTheAccount;
        }

        if (person is { AccountId: null })
        {
            person.LinkAccount(accountId);
            return person;
        }

        var account = (await identityService.ListAccountsAsync([accountId], cancellationToken)).SingleOrDefault()
            ?? throw new InvalidOperationException(AccountNotFoundMessage);
        var email = await userRepository.IsEmailTakenAsync(account.Email, 0, cancellationToken) ? null : account.Email;
        var user = User.CreateForAccount(accountId, account.FullName, email, now);
        userRepository.Add(user);
        return user;
    }
}
