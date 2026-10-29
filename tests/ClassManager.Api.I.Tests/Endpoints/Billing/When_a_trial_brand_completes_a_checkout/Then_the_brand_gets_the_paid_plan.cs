using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_trial_brand_completes_a_checkout;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_gets_the_paid_plan(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_brand_gets_the_paid_plan_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var externalSubscriptionId = BillingWebhookRequests.NewExternalSubscriptionId();
        fixture.ApiFactory.BillingGateway.Set(BillingWebhookRequests.ExternalPro(externalSubscriptionId, business.Business.OrganizationId));
        using var stripe = fixture.ApiFactory.CreateClient();

        using var response = await stripe.PostStripeEventAsync(BillingWebhookRequests.CheckoutSessionCompleted, externalSubscriptionId);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var current = await fixture.CurrentSubscriptionAsync(business);
        current.PlanCode.ShouldBe(PlanCodes.Pro);
        current.Price.ShouldBe(BillingWebhookRequests.ProPrice);
        current.ExternalSubscriptionId.ShouldBe(externalSubscriptionId);
        current.PaymentStatus.ShouldBe(SubscriptionPaymentStatus.Current);
    }
}
