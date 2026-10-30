using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_an_operator_withdraws_the_offer;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_shows_no_offer(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_the_brand_shows_no_offer_Run()
    {
        var changes = await BrandChanges.InSpainAsync(
            fixture,
            brandId => [Subscription.Start(brandId, "free", 0m, BrandChanges.Currency, null, BackofficeApiFactory.Now.AddDays(-3))]);
        using var offered = await changes.OfferPlanAsync("lite", 10m);

        using var response = await changes.WithdrawOfferAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await changes.GetBrandAsync()).Offer.ShouldBeNull();
    }
}
