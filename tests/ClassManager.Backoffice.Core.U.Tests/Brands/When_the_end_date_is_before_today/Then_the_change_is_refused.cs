using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_the_end_date_is_before_today;

public sealed class Then_the_change_is_refused
{
    [Fact]
    public async Task Then_the_change_is_refused_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.WithCurrentSubscription();

        var result = await scenario.ChangePlanAsync(PlanCodes.Pro, SubscriptionChangeScenario.Today.AddDays(-1));

        result.Error!.Code.ShouldBe(BrandErrorCodes.EndDateInThePast);
        scenario.Repository.Subscriptions.ShouldHaveSingleItem();
    }
}
