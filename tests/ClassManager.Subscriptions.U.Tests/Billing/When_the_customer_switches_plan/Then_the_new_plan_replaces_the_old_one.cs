using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_the_customer_switches_plan;

public sealed class Then_the_new_plan_replaces_the_old_one
{
    [Fact]
    public void Then_the_new_plan_replaces_the_old_one_Run()
    {
        var lite = Paying(LitePlanCode, LitePrice);

        var result = Sync(lite, External(ProPlanCode, ProPrice));

        lite.IsDeleted.ShouldBeTrue();
        result.Started.ShouldNotBeNull();
        result.Started.PlanCode.ShouldBe(ProPlanCode);
        result.Started.ExternalSubscriptionId.ShouldBe(ExternalSubscriptionId);
    }
}
