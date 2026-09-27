using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_billing_plan_uses_business_fee;

public sealed class Then_the_default_fee_is_charged
{
    [Fact]
    public void Then_the_default_fee_is_charged_Run()
    {
        var fee = BillingPlan.BusinessFee.MonthlyFee(defaultFee: 25000m);

        fee.ShouldBe(25000m);
        new BillingPlan(BillingPlanKind.CustomFee, 18000m).MonthlyFee(25000m).ShouldBe(18000m);
    }
}
