using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_new_client_joins_a_business_charging_a_monthly_fee;

public sealed class Then_the_client_pays_the_business_fee
{
    [Fact]
    public void Then_the_client_pays_the_business_fee_Run()
    {
        var defaultFeeChanges = new[]
        {
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-01").Value!, BusinessBillingMode.MonthlyFee, 25000m, TestData.Today, TestData.Now).Value!.WithId(),
        };

        var planChanges = ClientBillingPlanChange.ForNewClient(TestData.Client(), defaultFeeChanges, TestData.Today, TestData.Now);

        planChanges.Single().Plan.ShouldBe(BillingPlan.BusinessFee);
    }
}
