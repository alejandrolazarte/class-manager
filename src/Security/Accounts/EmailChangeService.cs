using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using ClassManager.Security.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClassManager.Security.Accounts;

internal sealed class EmailChangeService(
    SecurityDbContext securityContext,
    UserManager<ApplicationUser> userManager,
    IOptions<EmailChangeOptions> options,
    TimeProvider timeProvider)
    : IEmailChangeService
{
    private const int TokenByteCount = 32;

    public async Task<EmailChangeRequest> RequestAsync(
        Guid userId,
        string currentPassword,
        string newEmail,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return EmailChangeRequest.UnknownUser;
        }

        if (!await userManager.CheckPasswordAsync(user, currentPassword))
        {
            return EmailChangeRequest.InvalidPassword;
        }

        if (string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
        {
            return EmailChangeRequest.SameEmail;
        }

        if (await userManager.FindByEmailAsync(newEmail) is not null)
        {
            return EmailChangeRequest.EmailTaken;
        }

        var now = timeProvider.GetUtcNow();
        var value = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(TokenByteCount));
        securityContext.EmailChangeTokens.Add(new EmailChangeToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            NewEmail = newEmail,
            TokenHash = Hash(value),
            CreatedAt = now,
            ExpiresAt = now + options.Value.TokenLifetime,
        });
        await securityContext.SaveChangesAsync(cancellationToken);

        return EmailChangeRequest.Requested(value, user.Email ?? string.Empty);
    }

    public async Task<EmailChange> ConfirmAsync(string token, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var tokenHash = Hash(token);
        var storedToken = await securityContext.EmailChangeTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(changeToken => changeToken.TokenHash == tokenHash, cancellationToken);
        if (storedToken is null || storedToken.UsedAt is not null || storedToken.ExpiresAt <= now)
        {
            return EmailChange.InvalidToken;
        }

        var user = await userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user is null)
        {
            return EmailChange.InvalidToken;
        }

        if (await userManager.FindByEmailAsync(storedToken.NewEmail) is not null)
        {
            return EmailChange.EmailTaken;
        }

        var usedTokenCount = await securityContext.EmailChangeTokens
            .Where(changeToken => changeToken.UserId == user.Id && changeToken.UsedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(changeToken => changeToken.UsedAt, now), cancellationToken);
        if (usedTokenCount == 0)
        {
            return EmailChange.InvalidToken;
        }

        var previousEmail = user.Email ?? string.Empty;
        user.Email = storedToken.NewEmail;
        user.UserName = storedToken.NewEmail;
        user.EmailConfirmed = true;
        var update = await userManager.UpdateAsync(user);
        if (!update.Succeeded)
        {
            return EmailChange.EmailTaken;
        }

        return EmailChange.Changed(user.Id, previousEmail, storedToken.NewEmail);
    }

    private static string Hash(string tokenValue) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tokenValue)));
}
