using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Backoffice.Core.U.Tests.Brands.When_a_plan_is_offered_again;

public sealed class Then_it_replaces_the_current_offer
{
    private const decimal NewPrice = 15m;

    [Fact]
    public async Task Then_it_replaces_the_current_offer_Run()
    {
        var scenario = new SubscriptionChangeScenario();
        scenario.WithCurrentSubscription();
        var previous = scenario.WithOffer();

        await scenario.OfferPlanAsync(PlanCodes.Pro, NewPrice);

        previous.DeletedAt.ShouldBe(SubscriptionChangeScenario.Now);
        var current = scenario.Repository.Offers.Where(offer => !offer.IsDeleted).ShouldHaveSingleItem();
        current.PlanCode.ShouldBe(PlanCodes.Pro);
        current.Price.ShouldBe(NewPrice);
    }
}
