using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_a_checkout_completes_during_the_trial;

public sealed class Then_the_paid_plan_replaces_the_trial
{
    [Fact]
    public void Then_the_paid_plan_replaces_the_trial_Run()
    {
        var trial = Trial();

        var result = Sync(trial, External(status: ExternalSubscriptionStatus.Trialing));

        trial.IsDeleted.ShouldBeTrue();
        result.Started.ShouldNotBeNull();
        result.Started.PlanCode.ShouldBe(ProPlanCode);
        result.Started.Price.ShouldBe(ProPrice);
        result.Started.ExternalSubscriptionId.ShouldBe(ExternalSubscriptionId);
        result.Started.IsActiveOn(Today).ShouldBeTrue();
    }
}
