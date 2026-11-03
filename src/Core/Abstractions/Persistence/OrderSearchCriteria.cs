using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Abstractions.Persistence;

public sealed record OrderSearchCriteria(
    long? ClientId,
    bool AwaitingPickupOnly,
    int Limit,
    bool OwingOnly = false,
    long? CreatorUserId = null,
    IReadOnlyCollection<long>? HiddenCreatorUserIds = null,
    BillingMonth? Month = null);
