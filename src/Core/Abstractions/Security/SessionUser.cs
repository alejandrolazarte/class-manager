using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Abstractions.Security;

public sealed record SessionUser(Guid UserId, string Email, Guid TenantId, BusinessRole Role);
