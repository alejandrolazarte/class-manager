namespace ClassManager.Core.Domain.Fees;

public static class StudentPlanTimeline
{
    public static StudentPlanChange? ChangeIn(IEnumerable<StudentPlanChange> changes, BillingMonth month) =>
        changes
            .Where(change => change.EffectiveFrom <= month.FirstDay)
            .MaxBy(change => change.EffectiveFrom);
}
