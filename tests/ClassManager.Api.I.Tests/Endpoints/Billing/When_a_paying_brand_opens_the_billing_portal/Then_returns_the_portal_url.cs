using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_paying_brand_opens_the_billing_portal;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_the_portal_url(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_the_portal_url_Run()
    {
        var brand = await fixture.SignUpBrandAsync();
        using var checkoutResponse = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Pro);
        var externalSubscriptionId = BillingWebhookRequests.NewExternalSubscriptionId();
        fixture.ApiFactory.BillingGateway.Set(BillingWebhookRequests.ExternalPro(externalSubscriptionId, brand.OrganizationId));
        using var stripe = fixture.ApiFactory.CreateClient();
        using var webhookResponse = await stripe.PostStripeEventAsync(BillingWebhookRequests.CheckoutSessionCompleted, externalSubscriptionId);

        using var response = await brand.HttpClient.PostBillingPortalAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var customerId = fixture.ApiFactory.BillingGateway.CheckoutsFor(brand.OrganizationId).Single().ExternalCustomerId;
        (await response.ReadRedirectAsync()).Url.ShouldBe(new Uri(FakeBillingGateway.PortalHost + customerId));
    }
}
