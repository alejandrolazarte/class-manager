using ClassManager.Core.Common;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_the_brand_has_no_offer;

public sealed class Then_withdrawing_answers_not_found
{
    [Fact]
    public async Task Then_withdrawing_answers_not_found_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.WithCurrentSubscription();

        var result = await scenario.WithdrawOfferAsync();

        result.Error!.Kind.ShouldBe(ErrorKind.NotFound);
        result.Error.Code.ShouldBe(BrandErrorCodes.OfferNotFound);
    }
}
