namespace ClassManager.Security.Tokens;

/// <summary>
/// <paramref name="Kind"/> is a label chosen by the application that separates kinds of sessions (for example team
/// members and customers). It is fixed when the session starts and kept on every refresh, so a session can never turn
/// into another kind; <see langword="null"/> means the application's default kind.
/// </summary>
public sealed record TokenSubject(Guid UserId, string Email, Guid TenantId, string Role, string? Kind = null);
