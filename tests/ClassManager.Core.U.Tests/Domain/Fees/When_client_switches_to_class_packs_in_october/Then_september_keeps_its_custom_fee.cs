using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_client_switches_to_class_packs_in_october;

public sealed class Then_september_keeps_its_custom_fee
{
    [Fact]
    public void Then_september_keeps_its_custom_fee_Run()
    {
        var clientId = Guid.CreateVersion7();
        var changes = new[]
        {
            ClientBillingPlanChange.Create(clientId, BillingMonth.Parse("2026-10").Value!, BillingPlanKind.ClassPacks, null, TestData.Today, TestData.Now).Value!,
            ClientBillingPlanChange.Create(clientId, BillingMonth.Parse("2026-01").Value!, BillingPlanKind.CustomFee, 20000m, TestData.Today, TestData.Now).Value!,
        };

        var septemberPlan = FeeTimeline.PlanIn(changes, BillingMonth.Parse("2026-09").Value!);

        septemberPlan.ShouldBe(new BillingPlan(BillingPlanKind.CustomFee, 20000m));
        FeeTimeline.PlanIn(changes, BillingMonth.Parse("2026-10").Value!).Kind.ShouldBe(BillingPlanKind.ClassPacks);
    }
}
