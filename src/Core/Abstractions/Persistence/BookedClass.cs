namespace ClassManager.Core.Abstractions.Persistence;

public sealed record BookedClass(Guid StudentId, Guid ClassGroupId, DateOnly Date, bool IsCancelled);
