using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_the_plan_is_changed;

public sealed class Then_the_current_subscription_is_replaced
{
    [Fact]
    public async Task Then_the_current_subscription_is_replaced_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        var trial = scenario.WithCurrentSubscription();

        var result = await scenario.ChangePlanAsync(PlanCodes.Pro);

        var current = scenario.Repository.Subscriptions.Where(subscription => !subscription.IsDeleted).ShouldHaveSingleItem();
        current.Id.ShouldBe(result.Value!.SubscriptionId);
        current.PlanCode.ShouldBe(PlanCodes.Pro);
        trial.DeletedAt.ShouldBe(SubscriptionChangeScenario.Now);
        scenario.UnitOfWork.SaveCount.ShouldBe(1);
    }
}
