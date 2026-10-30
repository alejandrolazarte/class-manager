using ClassManager.Core.Common;
using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_enterprise_is_offered;

public sealed class Then_the_plan_field_is_refused
{
    [Fact]
    public async Task Then_the_plan_field_is_refused_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.WithCurrentSubscription();

        var result = await scenario.OfferPlanAsync(PlanCodes.Enterprise);

        result.Error!.Kind.ShouldBe(ErrorKind.Validation);
        result.Error.Code.ShouldBe(BrandErrorCodes.PlanNotOfferable);
        scenario.Repository.Offers.ShouldBeEmpty();
    }
}
