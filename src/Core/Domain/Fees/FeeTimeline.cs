namespace ClassManager.Core.Domain.Fees;

public static class FeeTimeline
{
    public static decimal? DefaultFeeIn(IEnumerable<DefaultMonthlyFeeChange> changes, BillingMonth month) =>
        changes
            .Where(change => change.EffectiveFrom <= month.FirstDay)
            .MaxBy(change => change.EffectiveFrom)?
            .Amount;

    public static BillingPlan PlanIn(IEnumerable<ClientBillingPlanChange> changes, BillingMonth month) =>
        changes
            .Where(change => change.EffectiveFrom <= month.FirstDay)
            .MaxBy(change => change.EffectiveFrom)?
            .Plan
        ?? BillingPlan.BusinessFee;
}
