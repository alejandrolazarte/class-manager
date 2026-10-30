using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_another_brand_has_an_offer;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_checkout_charges_the_list_price(ApiFixture fixture)
{
    private const decimal LiteListPrice = 19m;

    [Fact]
    public async Task Then_the_checkout_charges_the_list_price_Run()
    {
        var offeredBrand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await fixture.OfferPlanAsync(offeredBrand.OrganizationId, PlanCodes.Lite);
        var otherBrand = await fixture.SignUpBrandAsync();

        using var response = await otherBrand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Lite);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        fixture.ApiFactory.BillingGateway.CheckoutsFor(otherBrand.OrganizationId).Single().Plan.UnitAmount.ShouldBe(LiteListPrice);
    }
}
