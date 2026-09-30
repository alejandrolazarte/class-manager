namespace ClassManager.Core.Domain.Makeups;

public sealed record MakeupCredit(DateOnly MissedOn, MakeupReason Reason, DateOnly ExpiresOn);
