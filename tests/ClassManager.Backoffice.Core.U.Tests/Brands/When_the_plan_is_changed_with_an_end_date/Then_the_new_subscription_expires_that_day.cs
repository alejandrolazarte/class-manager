using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_the_plan_is_changed_with_an_end_date;

public sealed class Then_the_new_subscription_expires_that_day
{
    [Fact]
    public async Task Then_the_new_subscription_expires_that_day_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.WithCurrentSubscription();
        var endDate = SubscriptionChangeScenario.Today.AddMonths(3);

        await scenario.ChangePlanAsync(PlanCodes.Pro, endDate);

        scenario.Repository.Subscriptions.Single(subscription => !subscription.IsDeleted).ExpiredOn.ShouldBe(endDate);
    }
}
