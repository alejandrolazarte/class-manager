namespace ClassManager.Core.Domain.Fees;

public sealed record BillingPlan(BillingPlanKind Kind, decimal? CustomFee)
{
    public static readonly BillingPlan BusinessFee = new(BillingPlanKind.BusinessFee, null);
    public static readonly BillingPlan ClassPacks = new(BillingPlanKind.ClassPacks, null);

    public static BillingPlan ForBusiness(BusinessBillingMode? billingMode) =>
        billingMode == BusinessBillingMode.ClassPacks ? ClassPacks : BusinessFee;

    public bool PaysPerClass => Kind == BillingPlanKind.ClassPacks;

    public decimal? MonthlyFee(decimal? defaultFee) => Kind switch
    {
        BillingPlanKind.CustomFee => CustomFee,
        BillingPlanKind.ClassPacks => null,
        _ => defaultFee,
    };
}
