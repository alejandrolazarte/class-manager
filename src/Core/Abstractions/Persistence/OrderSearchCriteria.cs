namespace ClassManager.Core.Abstractions.Persistence;

public sealed record OrderSearchCriteria(Guid? ClientId, bool AwaitingPickupOnly, int Limit, bool RequestedOnly = false);
