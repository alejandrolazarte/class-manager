using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using ClassManager.Security.Accounts;
using ClassManager.Security.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClassManager.Security.Tokens;

internal sealed class JwtTokenIssuer(
    SecurityDbContext securityContext,
    IAccessTokenFactory accessTokenFactory,
    UserManager<ApplicationUser> userManager,
    ITokenSubjectResolver tokenSubjectResolver,
    IOptions<JwtOptions> options,
    TimeProvider timeProvider)
    : ITokenIssuer
{
    private const int RefreshTokenByteCount = 32;

    public async Task<IssuedTokenPair> IssueAsync(TokenSubject subject, CancellationToken cancellationToken)
    {
        var accessToken = accessTokenFactory.Create(subject);
        var (refreshToken, refreshTokenValue) = CreateRefreshToken(subject.UserId, Guid.CreateVersion7());

        securityContext.RefreshTokens.Add(refreshToken);
        await securityContext.SaveChangesAsync(cancellationToken);

        return new IssuedTokenPair(accessToken.Token, accessToken.ExpiresAt, refreshTokenValue, refreshToken.ExpiresAt);
    }

    public async Task<IssuedTokenPair?> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var tokenHash = Hash(refreshToken);
        var storedToken = await securityContext.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null || storedToken.ExpiresAt <= now)
        {
            return null;
        }

        if (storedToken.RevokedAt is not null)
        {
            if (storedToken.ReplacedByTokenId is not null)
            {
                await RevokeAllOfUserAsync(storedToken.UserId, now, cancellationToken);
            }

            return null;
        }

        var subject = await FindTokenSubjectAsync(storedToken.UserId, cancellationToken);
        if (subject is null)
        {
            return null;
        }

        var replacementId = Guid.CreateVersion7();
        var rotatedTokenCount = await securityContext.RefreshTokens
            .Where(token => token.Id == storedToken.Id && token.RevokedAt == null)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(token => token.RevokedAt, now)
                    .SetProperty(token => token.ReplacedByTokenId, replacementId),
                cancellationToken);
        if (rotatedTokenCount == 0)
        {
            await RevokeAllOfUserAsync(storedToken.UserId, now, cancellationToken);
            return null;
        }

        var accessToken = accessTokenFactory.Create(subject);
        var (replacementToken, replacementTokenValue) = CreateRefreshToken(storedToken.UserId, replacementId);
        securityContext.RefreshTokens.Add(replacementToken);
        await securityContext.SaveChangesAsync(cancellationToken);

        return new IssuedTokenPair(accessToken.Token, accessToken.ExpiresAt, replacementTokenValue, replacementToken.ExpiresAt);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokenHash = Hash(refreshToken);
        var now = timeProvider.GetUtcNow();

        await securityContext.RefreshTokens
            .Where(token => token.TokenHash == tokenHash && token.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.RevokedAt, now), cancellationToken);
    }

    private async Task<TokenSubject?> FindTokenSubjectAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        return user?.Email is null
            ? null
            : await tokenSubjectResolver.ResolveAsync(user.Id, user.Email, cancellationToken);
    }

    private async Task RevokeAllOfUserAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken) =>
        await securityContext.RefreshTokens
            .Where(token => token.UserId == userId && token.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.RevokedAt, now), cancellationToken);

    private (RefreshToken Token, string Value) CreateRefreshToken(Guid userId, Guid refreshTokenId)
    {
        var now = timeProvider.GetUtcNow();
        var value = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(RefreshTokenByteCount));
        var token = new RefreshToken
        {
            Id = refreshTokenId,
            UserId = userId,
            TokenHash = Hash(value),
            CreatedAt = now,
            ExpiresAt = now + options.Value.RefreshTokenLifetime,
        };

        return (token, value);
    }

    private static string Hash(string refreshTokenValue) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshTokenValue)));
}
