namespace ClassManager.Core.Abstractions.Persistence;

public sealed record BookedMakeup(Guid StudentId, Guid ClassGroupId, DateOnly Date, bool IsCancelled);
