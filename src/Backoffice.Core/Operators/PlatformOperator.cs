using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.Operators;

public sealed record PlatformOperator(Guid AccountId, long UserId, long BusinessId, BusinessRole Role);
