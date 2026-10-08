namespace ClassManager.Core.Abstractions.Persistence;

public sealed record ClientPaidMonth(Guid ClientId, DateOnly Month);
