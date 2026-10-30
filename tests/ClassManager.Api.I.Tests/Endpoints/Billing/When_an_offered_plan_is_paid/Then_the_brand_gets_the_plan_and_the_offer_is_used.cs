using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Billing;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_an_offered_plan_is_paid;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_gets_the_plan_and_the_offer_is_used(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_brand_gets_the_plan_and_the_offer_is_used_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await fixture.OfferPlanAsync(brand.OrganizationId, PlanCodes.Lite);
        var externalSubscriptionId = BillingWebhookRequests.NewExternalSubscriptionId();
        fixture.ApiFactory.BillingGateway.Set(new ExternalSubscription(
            externalSubscriptionId,
            brand.OrganizationId,
            PlanCodes.Lite,
            SelfServiceBillingRequests.OfferedPrice,
            SelfServiceBillingRequests.Euro,
            ExternalSubscriptionStatus.Trialing,
            CancelOn: null,
            EndedOn: null));
        using var stripe = fixture.ApiFactory.CreateClient();

        using var response = await stripe.PostStripeEventAsync(BillingWebhookRequests.CheckoutSessionCompleted, externalSubscriptionId);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await fixture.CurrentOfferAsync(brand.OrganizationId)).ShouldBeNull();
        var current = await fixture.CurrentSubscriptionAsync(brand.OrganizationId);
        current.PlanCode.ShouldBe(PlanCodes.Lite);
        current.Price.ShouldBe(SelfServiceBillingRequests.OfferedPrice);
    }
}
