namespace ClassManager.Core.Abstractions.Security;

public sealed record SessionUser(Guid AccountId, string Email, Guid TenantId, string RoleName, string Kind = AccountKinds.Team);
