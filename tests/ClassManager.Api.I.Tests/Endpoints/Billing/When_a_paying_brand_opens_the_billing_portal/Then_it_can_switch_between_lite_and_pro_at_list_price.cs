using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_paying_brand_opens_the_billing_portal;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_can_switch_between_lite_and_pro_at_list_price(ApiFixture fixture)
{
    private const decimal LiteListPrice = 19m;
    private const decimal ProListPrice = 39m;

    [Fact]
    public async Task Then_it_can_switch_between_lite_and_pro_at_list_price_Run()
    {
        var brand = await fixture.SignUpBrandAsync();
        using var checkoutResponse = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Lite);
        var customerId = fixture.ApiFactory.BillingGateway.CheckoutsFor(brand.OrganizationId).Single().ExternalCustomerId;

        using var response = await brand.HttpClient.PostBillingPortalAsync();

        var switchablePlans = fixture.ApiFactory.BillingGateway.PortalsFor(customerId).Single().SwitchablePlans;
        switchablePlans.Select(plan => (plan.PlanCode, plan.UnitAmount))
            .ShouldBe([(PlanCodes.Lite, LiteListPrice), (PlanCodes.Pro, ProListPrice)]);
    }
}
