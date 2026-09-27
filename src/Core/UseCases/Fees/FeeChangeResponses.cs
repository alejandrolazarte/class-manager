using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record MonthlyFeeChangeResponse(string EffectiveFrom, decimal? Amount)
{
    public static IReadOnlyList<MonthlyFeeChangeResponse> From(IEnumerable<DefaultMonthlyFeeChange> changes) =>
        [.. changes
            .OrderBy(change => change.EffectiveFrom)
            .Select(change => new MonthlyFeeChangeResponse(BillingMonth.From(change.EffectiveFrom).ToString(), change.Amount))];
}

public sealed record BillingPlanResponse(BillingPlanKind Kind, decimal? CustomFee)
{
    public static BillingPlanResponse From(BillingPlan plan) => new(plan.Kind, plan.CustomFee);
}

public sealed record BillingPlanChangeResponse(string EffectiveFrom, BillingPlanKind Kind, decimal? CustomFee)
{
    public static IReadOnlyList<BillingPlanChangeResponse> From(IEnumerable<ClientBillingPlanChange> changes) =>
        [.. changes
            .OrderBy(change => change.EffectiveFrom)
            .Select(change => new BillingPlanChangeResponse(BillingMonth.From(change.EffectiveFrom).ToString(), change.Kind, change.CustomFee))];
}

public sealed record ClientBillingResponse(BillingPlanResponse BillingPlan, IReadOnlyList<BillingPlanChangeResponse> BillingPlanChanges)
{
    public static ClientBillingResponse From(IReadOnlyCollection<ClientBillingPlanChange> changes, DateOnly today) =>
        new(BillingPlanResponse.From(FeeTimeline.PlanIn(changes, BillingMonth.From(today))), BillingPlanChangeResponse.From(changes));
}
