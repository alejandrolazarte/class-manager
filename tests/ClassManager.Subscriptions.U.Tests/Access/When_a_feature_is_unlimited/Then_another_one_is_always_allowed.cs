using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_a_feature_is_unlimited;

public sealed class Then_another_one_is_always_allowed
{
    [Fact]
    public void Then_another_one_is_always_allowed_Run()
    {
        var features = Combine([Included(CountedFeatureCode)], []);

        features.AllowsAnother(CountedFeatureCode, currentCount: int.MaxValue - 1).ShouldBeTrue();
    }
}
