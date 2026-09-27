using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_client_has_no_billing_plan;

public sealed class Then_business_fee_applies
{
    [Fact]
    public void Then_business_fee_applies_Run()
    {
        var plan = FeeTimeline.PlanIn([], BillingMonth.From(TestData.Today));

        plan.ShouldBe(BillingPlan.BusinessFee);
    }
}
