using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using ClassManager.Security.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace ClassManager.Security.Accounts;

internal sealed class PasswordResetService(
    SecurityDbContext securityContext,
    UserManager<ApplicationUser> userManager,
    IOptions<PasswordResetOptions> options,
    TimeProvider timeProvider)
    : IPasswordResetService
{
    private const int TokenByteCount = 32;

    public async Task<string?> CreateTokenAsync(string email, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return null;
        }

        var now = timeProvider.GetUtcNow();
        var value = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(TokenByteCount));
        securityContext.PasswordResetTokens.Add(new PasswordResetToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            TokenHash = Hash(value),
            CreatedAt = now,
            ExpiresAt = now + options.Value.TokenLifetime,
        });
        await securityContext.SaveChangesAsync(cancellationToken);

        return value;
    }

    public async Task<PasswordReset> ResetAsync(string token, string newPassword, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var tokenHash = Hash(token);
        var storedToken = await securityContext.PasswordResetTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(resetToken => resetToken.TokenHash == tokenHash, cancellationToken);
        if (storedToken is null || storedToken.UsedAt is not null || storedToken.ExpiresAt <= now)
        {
            return PasswordReset.InvalidToken;
        }

        var user = await userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user is null)
        {
            return PasswordReset.InvalidToken;
        }

        var passwordProblem = await FindPasswordProblemAsync(user, newPassword);
        if (passwordProblem is not null)
        {
            return PasswordReset.InvalidPassword(passwordProblem);
        }

        await using var ownTransaction = await BeginTransactionUnlessOneIsOpenAsync(cancellationToken);
        var usedTokenCount = await securityContext.PasswordResetTokens
            .Where(resetToken => resetToken.UserId == user.Id && resetToken.UsedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(resetToken => resetToken.UsedAt, now), cancellationToken);
        if (usedTokenCount == 0)
        {
            return PasswordReset.InvalidToken;
        }

        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, newPassword);
        user.AccessFailedCount = 0;
        user.LockoutEnd = null;
        await userManager.UpdateSecurityStampAsync(user);
        await securityContext.RefreshTokens
            .Where(refreshToken => refreshToken.UserId == user.Id && refreshToken.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(refreshToken => refreshToken.RevokedAt, now), cancellationToken);
        if (ownTransaction is not null)
        {
            await ownTransaction.CommitAsync(cancellationToken);
        }

        return PasswordReset.Succeeded;
    }

    private async Task<IDbContextTransaction?> BeginTransactionUnlessOneIsOpenAsync(CancellationToken cancellationToken) =>
        securityContext.Database.CurrentTransaction is null
            ? await securityContext.Database.BeginTransactionAsync(cancellationToken)
            : null;

    private async Task<string?> FindPasswordProblemAsync(ApplicationUser user, string newPassword)
    {
        foreach (var validator in userManager.PasswordValidators)
        {
            var validation = await validator.ValidateAsync(userManager, user, newPassword);
            if (!validation.Succeeded)
            {
                return validation.Errors.First().Description;
            }
        }

        return null;
    }

    private static string Hash(string tokenValue) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tokenValue)));
}
