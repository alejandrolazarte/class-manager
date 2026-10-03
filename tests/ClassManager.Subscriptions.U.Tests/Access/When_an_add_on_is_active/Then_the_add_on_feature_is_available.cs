using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_an_add_on_is_active;

public sealed class Then_the_add_on_feature_is_available
{
    [Fact]
    public void Then_the_add_on_feature_is_available_Run()
    {
        var features = Combine([Included(IncludedFeatureCode)], [AddOn(AddOnFeatureCode, Today.AddMonths(-1), Today)]);

        features.Has(AddOnFeatureCode).ShouldBeTrue();
    }
}
