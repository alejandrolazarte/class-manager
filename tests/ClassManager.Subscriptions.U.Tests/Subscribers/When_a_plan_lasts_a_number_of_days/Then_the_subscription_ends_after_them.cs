using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_a_plan_lasts_a_number_of_days;

public sealed class Then_the_subscription_ends_after_them
{
    private const int TrialDays = 30;

    [Fact]
    public void Then_the_subscription_ends_after_them_Run()
    {
        var plan = Plan.Create(PlanCode, displayOrder: 1, 0m, Currency, BillingPeriod.Monthly, durationInDays: TrialDays);

        var subscription = Subscription.StartAtListPrice(SubscriberId, plan, Now);

        subscription.IsActiveOn(Today.AddDays(TrialDays - 1)).ShouldBeTrue();
        subscription.IsActiveOn(Today.AddDays(TrialDays)).ShouldBeFalse();
    }
}
