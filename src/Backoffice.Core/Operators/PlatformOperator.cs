using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.Operators;

public sealed record PlatformOperator(Guid AccountId, Guid UserId, Guid BusinessId, BusinessRole Role);
