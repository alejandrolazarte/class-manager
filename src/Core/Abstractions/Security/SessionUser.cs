namespace ClassManager.Core.Abstractions.Security;

public sealed record SessionUser(Guid UserId, string Email, Guid TenantId, string RoleName, string Kind = AccountKinds.Team);
