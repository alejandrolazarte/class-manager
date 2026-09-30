namespace ClassManager.Core.Abstractions.Persistence;

public sealed record NotifiedAbsence(Guid StudentId, Guid ClassGroupId, DateOnly Date);
