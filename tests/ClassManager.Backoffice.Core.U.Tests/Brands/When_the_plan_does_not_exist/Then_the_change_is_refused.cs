using ClassManager.Core.Common;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_the_plan_does_not_exist;

public sealed class Then_the_change_is_refused
{
    [Fact]
    public async Task Then_the_change_is_refused_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.WithCurrentSubscription();

        var result = await scenario.ChangePlanAsync("gold");

        result.Error!.Kind.ShouldBe(ErrorKind.Validation);
        result.Error.Code.ShouldBe(BrandErrorCodes.UnknownPlan);
        scenario.UnitOfWork.SaveCount.ShouldBe(0);
    }
}
