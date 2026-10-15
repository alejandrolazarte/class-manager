namespace ClassManager.Security.Tokens;

public sealed record LastTokenSession(Guid TenantId, string? Kind);
