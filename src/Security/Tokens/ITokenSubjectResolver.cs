namespace ClassManager.Security.Tokens;

/// <summary>
/// Implemented by the application: rebuilds who a user is (tenant and role) when a refresh token is exchanged.
/// Returns <see langword="null"/> when the user no longer belongs to any tenant, which ends the session.
/// </summary>
public interface ITokenSubjectResolver
{
    Task<TokenSubject?> ResolveAsync(Guid userId, string email, CancellationToken cancellationToken);
}
