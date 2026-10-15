using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Core.Emails;

public static class RecipientEmailTexts
{
    public static async Task<IEmailTexts> OfUserAsync(IIdentityService identityService, Guid userId, CancellationToken cancellationToken)
    {
        var accounts = await identityService.ListAccountsAsync([userId], cancellationToken);
        return EmailTexts.For(accounts.Count == 0 ? null : accounts[0].PreferredLanguage);
    }

    public static async Task<IEmailTexts> OfEmailAsync(IIdentityService identityService, string email, CancellationToken cancellationToken) =>
        await identityService.FindUserIdByEmailAsync(email, cancellationToken) is { } userId
            ? await OfUserAsync(identityService, userId, cancellationToken)
            : EmailTexts.Spanish;
}
