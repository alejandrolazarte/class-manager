namespace ClassManager.Core.Abstractions.Persistence;

public sealed record NotifiedAbsence(long StudentId, long ClassGroupId, DateOnly Date);
