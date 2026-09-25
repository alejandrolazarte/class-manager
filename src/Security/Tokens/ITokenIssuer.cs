namespace ClassManager.Security.Tokens;

public interface ITokenIssuer
{
    Task<IssuedTokenPair> IssueAsync(TokenSubject subject, CancellationToken cancellationToken);

    /// <summary>
    /// Rotates a refresh token: the given one is revoked and a new pair is issued.
    /// Returns <see langword="null"/> when the token is unknown, expired or revoked; reusing an already rotated token
    /// also revokes every session of its user.
    /// </summary>
    Task<IssuedTokenPair?> RefreshAsync(string refreshToken, CancellationToken cancellationToken);

    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken);
}
