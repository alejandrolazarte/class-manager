using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.UseCases;

internal static class SignInEmails
{
    public static async Task<string?> FirstAsync(
        IIdentityService identityService,
        IReadOnlyList<Guid> accountIds,
        CancellationToken cancellationToken)
    {
        if (accountIds.Count == 0)
        {
            return null;
        }

        var accounts = await identityService.ListAccountsAsync(accountIds, cancellationToken);
        return accountIds
            .Select(accountId => accounts.FirstOrDefault(account => account.UserId == accountId)?.Email)
            .FirstOrDefault(email => email is not null);
    }
}
