namespace ClassManager.Security.Tokens;

/// <summary>
/// Implemented by the application: rebuilds who a user is (tenant and role) when a refresh token is exchanged.
/// With a tenant id, resolves the user in that tenant only; without one, in the user's default tenant.
/// The kind is the one the refresh token was issued with and must be resolved as that kind only.
/// Returns <see langword="null"/> when the user has no access, which ends the session (or refuses a tenant switch).
/// </summary>
public interface ITokenSubjectResolver
{
    Task<TokenSubject?> ResolveAsync(Guid userId, string email, Guid? tenantId, string? kind, CancellationToken cancellationToken);
}
