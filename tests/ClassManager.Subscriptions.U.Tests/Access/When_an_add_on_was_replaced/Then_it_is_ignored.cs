using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Access.When_an_add_on_was_replaced;

public sealed class Then_it_is_ignored
{
    [Fact]
    public void Then_it_is_ignored_Run()
    {
        var replaced = AddOn(AddOnFeatureCode);
        replaced.Delete(Now);

        var features = Combine([], [replaced]);

        features.Has(AddOnFeatureCode).ShouldBeFalse();
    }
}
