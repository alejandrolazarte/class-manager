using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_a_counted_feature_is_missing;

public sealed class Then_not_even_one_is_allowed
{
    [Fact]
    public void Then_not_even_one_is_allowed_Run()
    {
        var features = Combine([], []);

        features.AllowsAnother(CountedFeatureCode, currentCount: 0).ShouldBeFalse();
    }
}
