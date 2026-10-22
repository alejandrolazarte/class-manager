namespace ClassManager.Core.Abstractions.Persistence;

public sealed record BookedClass(long StudentId, long ClassGroupId, DateOnly Date, bool IsCancelled);
