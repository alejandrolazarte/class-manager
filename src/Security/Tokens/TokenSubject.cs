namespace ClassManager.Security.Tokens;

public sealed record TokenSubject(Guid UserId, string Email, Guid TenantId, string Role);
