using static ClassManager.Subscriptions.U.Tests.SubscriptionsTestData;

namespace ClassManager.Subscriptions.U.Tests.Subscribers.When_a_removed_add_on_is_restored;

public sealed class Then_it_counts_again
{
    [Fact]
    public void Then_it_counts_again_Run()
    {
        var addOn = AddOn(AddOnFeatureCode);
        addOn.Delete(Now);

        addOn.Restore();

        Combine([], [addOn]).Has(AddOnFeatureCode).ShouldBeTrue();
        addOn.DeletedAt.ShouldBeNull();
    }
}
