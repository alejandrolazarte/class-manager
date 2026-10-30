using ClassManager.Core.Common;
using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_the_brand_pays_through_stripe;

public sealed class Then_the_plan_change_is_refused
{
    [Fact]
    public async Task Then_the_plan_change_is_refused_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        var paying = scenario.WithStripeSubscription();

        var result = await scenario.ChangePlanAsync(PlanCodes.Free);

        result.Error!.Kind.ShouldBe(ErrorKind.Conflict);
        result.Error.Code.ShouldBe(BrandErrorCodes.SubscriptionExternal);
        paying.IsDeleted.ShouldBeFalse();
        scenario.UnitOfWork.SaveCount.ShouldBe(0);
    }
}
