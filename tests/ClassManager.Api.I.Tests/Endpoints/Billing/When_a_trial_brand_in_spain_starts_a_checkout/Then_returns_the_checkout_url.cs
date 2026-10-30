using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Billing;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_trial_brand_in_spain_starts_a_checkout;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_the_checkout_url(ApiFixture fixture)
{
    private const decimal ProListPrice = 39m;

    [Fact]
    public async Task Then_returns_the_checkout_url_Run()
    {
        var brand = await fixture.SignUpBrandAsync();

        using var response = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Pro);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.ReadRedirectAsync()).Url.ToString().ShouldStartWith(FakeBillingGateway.CheckoutHost);
        var plan = fixture.ApiFactory.BillingGateway.CheckoutsFor(brand.OrganizationId).Single().Plan;
        plan.ShouldBe(new BillingPlanPrice(PlanCodes.Pro, "Laztty Pro", ProListPrice, SelfServiceBillingRequests.Euro));
    }
}
