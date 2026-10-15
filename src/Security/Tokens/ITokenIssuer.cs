namespace ClassManager.Security.Tokens;

public interface ITokenIssuer
{
    Task<IssuedTokenPair> IssueAsync(TokenSubject subject, CancellationToken cancellationToken);

    /// <summary>
    /// Rotates a refresh token: the given one is revoked and a new pair is issued for the same tenant, or for
    /// <paramref name="tenantId"/> when given (switching tenant).
    /// Returns <see langword="null"/> when the token is unknown, expired or revoked, or the user has no access to the
    /// tenant; reusing an already rotated token also revokes every session of its user.
    /// </summary>
    Task<IssuedTokenPair?> RefreshAsync(string refreshToken, Guid? tenantId, CancellationToken cancellationToken);

    Task<IssuedTokenPair?> SwitchAsync(string refreshToken, Guid tenantId, string? kind, CancellationToken cancellationToken);

    /// <summary>
    /// The tenant and kind of the most recent session issued to the user, signed out or not, so a new sign-in can
    /// resume where the user left off. Returns <see langword="null"/> when no session had a tenant.
    /// </summary>
    Task<LastTokenSession?> FindLastSessionAsync(Guid userId, CancellationToken cancellationToken);

    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken);
}
