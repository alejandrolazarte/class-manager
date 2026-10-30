namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_the_offer_is_withdrawn;

public sealed class Then_it_is_deleted
{
    [Fact]
    public async Task Then_it_is_deleted_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.WithCurrentSubscription();
        var offer = scenario.WithOffer();

        var result = await scenario.WithdrawOfferAsync();

        offer.DeletedAt.ShouldBe(SubscriptionChangeScenario.Now);
        result.Value!.Price.ShouldBe(SubscriptionChangeScenario.OfferedPrice);
        scenario.UnitOfWork.SaveCount.ShouldBe(1);
    }
}
