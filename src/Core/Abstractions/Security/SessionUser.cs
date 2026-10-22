namespace ClassManager.Core.Abstractions.Security;

public sealed record SessionUser(Guid AccountId, string Email, long TenantId, string RoleName, string Kind = AccountKinds.Team);
