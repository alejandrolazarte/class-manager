namespace ClassManager.Core.Domain.Fees;

public static class FeeTimeline
{
    public static decimal? DefaultFeeIn(IEnumerable<DefaultMonthlyFeeChange> changes, BillingMonth month) =>
        DefaultFeeChangeIn(changes, month)?.Amount;

    public static BusinessBillingMode? BillingModeIn(IEnumerable<DefaultMonthlyFeeChange> changes, BillingMonth month) =>
        DefaultFeeChangeIn(changes, month)?.BillingMode;

    public static BillingPlan PlanIn(
        IEnumerable<ClientBillingPlanChange> changes,
        IEnumerable<DefaultMonthlyFeeChange> defaultFeeChanges,
        BillingMonth month) =>
        changes
            .Where(change => change.EffectiveFrom <= month.FirstDay)
            .MaxBy(change => change.EffectiveFrom)?
            .Plan
        ?? BillingPlan.ForBusiness(BillingModeIn(defaultFeeChanges, month));

    private static DefaultMonthlyFeeChange? DefaultFeeChangeIn(IEnumerable<DefaultMonthlyFeeChange> changes, BillingMonth month) =>
        changes
            .Where(change => change.EffectiveFrom <= month.FirstDay)
            .MaxBy(change => change.EffectiveFrom);
}
