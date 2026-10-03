using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_features_come_from_an_active_subscription;

public sealed class Then_the_subscription_is_active
{
    [Fact]
    public void Then_the_subscription_is_active_Run()
    {
        var features = Combine([Included(IncludedFeatureCode)], []);

        features.IsActive.ShouldBeTrue();
    }
}
