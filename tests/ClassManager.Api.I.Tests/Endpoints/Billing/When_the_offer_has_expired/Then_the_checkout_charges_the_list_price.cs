using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_the_offer_has_expired;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_checkout_charges_the_list_price(ApiFixture fixture)
{
    private const decimal LiteListPrice = 19m;

    [Fact]
    public async Task Then_the_checkout_charges_the_list_price_Run()
    {
        var brand = await fixture.SignUpBrandAsync();
        var yesterday = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddDays(-1);
        await fixture.OfferPlanAsync(brand.OrganizationId, PlanCodes.Lite, expiredOn: yesterday, createdDaysAgo: 5);

        using var response = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Lite);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        fixture.ApiFactory.BillingGateway.CheckoutsFor(brand.OrganizationId).Single().Plan.UnitAmount.ShouldBe(LiteListPrice);
    }
}
