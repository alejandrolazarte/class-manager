using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_an_add_on_has_not_started;

public sealed class Then_the_add_on_is_ignored
{
    [Fact]
    public void Then_the_add_on_is_ignored_Run()
    {
        var features = Combine([], [AddOn(AddOnFeatureCode, Today.AddDays(1))]);

        features.Has(AddOnFeatureCode).ShouldBeFalse();
    }
}
