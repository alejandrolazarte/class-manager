using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_an_operator_offers_a_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_shows_the_offer_with_its_payment_link(BackofficeApiFixture fixture)
{
    private const string PaymentLink = BackofficeApiFactory.BusinessWebAppUrl + "/settings/plan?offer=1";

    [Fact]
    public async Task Then_the_brand_shows_the_offer_with_its_payment_link_Run()
    {
        var changes = await BrandChanges.InSpainAsync(
            fixture,
            brandId => [Subscription.Start(brandId, "free", 0m, BrandChanges.Currency, null, BackofficeApiFactory.Now.AddDays(-3))]);

        using var response = await changes.OfferPlanAsync("lite", 10m);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var brand = await changes.GetBrandAsync();
        brand.Offer.ShouldBe(new BrandOfferResponse("lite", 10m, BrandChanges.Currency, IsActive: true, PaymentLink));
        brand.Subscription!.PlanCode.ShouldBe("free");
    }
}
