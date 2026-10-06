using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.UseCases.Clients;

internal static class SignInEmails
{
    public static async Task<string?> FirstAsync(
        IIdentityService identityService,
        IReadOnlyList<Guid> userIds,
        CancellationToken cancellationToken)
    {
        if (userIds.Count == 0)
        {
            return null;
        }

        var accounts = await identityService.ListAccountsAsync(userIds, cancellationToken);
        return userIds
            .Select(userId => accounts.FirstOrDefault(account => account.UserId == userId)?.Email)
            .FirstOrDefault(email => email is not null);
    }
}
