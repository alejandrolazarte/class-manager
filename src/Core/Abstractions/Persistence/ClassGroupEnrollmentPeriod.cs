namespace ClassManager.Core.Abstractions.Persistence;

public sealed record ClassGroupEnrollmentPeriod(long ClassGroupId, DateOnly StartDate, DateOnly? EndDate)
{
    public bool IsActiveOn(DateOnly date) => StartDate <= date && (EndDate is null || EndDate >= date);
}
