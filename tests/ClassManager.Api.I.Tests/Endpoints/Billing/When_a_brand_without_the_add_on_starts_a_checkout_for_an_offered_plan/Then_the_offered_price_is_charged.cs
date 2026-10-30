using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_brand_without_the_add_on_starts_a_checkout_for_an_offered_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_offered_price_is_charged(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_offered_price_is_charged_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await fixture.OfferPlanAsync(brand.OrganizationId, PlanCodes.Lite);

        using var response = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Lite);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        fixture.ApiFactory.BillingGateway.CheckoutsFor(brand.OrganizationId).Single().Plan.UnitAmount.ShouldBe(SelfServiceBillingRequests.OfferedPrice);
    }
}
