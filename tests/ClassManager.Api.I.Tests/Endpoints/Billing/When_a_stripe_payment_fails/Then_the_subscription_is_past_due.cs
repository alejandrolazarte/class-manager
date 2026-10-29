using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Billing;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_stripe_payment_fails;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_subscription_is_past_due(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_subscription_is_past_due_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var organizationId = business.Business.OrganizationId;
        var externalSubscriptionId = BillingWebhookRequests.NewExternalSubscriptionId();
        var gateway = fixture.ApiFactory.BillingGateway;
        using var stripe = fixture.ApiFactory.CreateClient();
        gateway.Set(BillingWebhookRequests.ExternalPro(externalSubscriptionId, organizationId));
        using var checkoutResponse = await stripe.PostStripeEventAsync(BillingWebhookRequests.CheckoutSessionCompleted, externalSubscriptionId);
        gateway.Set(BillingWebhookRequests.ExternalPro(externalSubscriptionId, organizationId, ExternalSubscriptionStatus.PastDue));

        using var response = await stripe.PostStripeEventAsync(BillingWebhookRequests.InvoicePaymentFailed, externalSubscriptionId);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var current = await fixture.CurrentSubscriptionAsync(business);
        current.PaymentStatus.ShouldBe(SubscriptionPaymentStatus.PastDue);
        current.PlanCode.ShouldBe(PlanCodes.Pro);
    }
}
