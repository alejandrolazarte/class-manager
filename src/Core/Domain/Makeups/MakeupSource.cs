namespace ClassManager.Core.Domain.Makeups;

public sealed record MakeupSource(DateOnly MissedOn, MakeupReason Reason)
{
    public DateOnly ExpiresOn => MissedOn.AddDays(MakeupCredits.ValidityDays);
}
