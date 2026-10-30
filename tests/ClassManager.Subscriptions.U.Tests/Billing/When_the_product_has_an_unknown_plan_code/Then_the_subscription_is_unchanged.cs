using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_the_product_has_an_unknown_plan_code;

public sealed class Then_the_subscription_is_unchanged
{
    [Fact]
    public void Then_the_subscription_is_unchanged_Run()
    {
        var trial = Trial();

        var result = Sync(trial, External(planCode: "premium"));

        result.Outcome.ShouldBe(ExternalSubscriptionSyncOutcome.UnknownPlan);
        trial.IsDeleted.ShouldBeFalse();
    }
}
