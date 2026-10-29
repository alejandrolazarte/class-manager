using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_the_same_state_arrives_twice;

public sealed class Then_the_subscription_is_not_replaced
{
    [Fact]
    public void Then_the_subscription_is_not_replaced_Run()
    {
        var paying = Paying();

        var result = Sync(paying, External());

        result.Outcome.ShouldBe(ExternalSubscriptionSyncOutcome.Applied);
        result.Started.ShouldBeNull();
        paying.IsDeleted.ShouldBeFalse();
    }
}
