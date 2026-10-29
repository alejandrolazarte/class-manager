using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_stripe_sends_the_same_event_twice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_applied_once(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_applied_once_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var externalSubscriptionId = BillingWebhookRequests.NewExternalSubscriptionId();
        fixture.ApiFactory.BillingGateway.Set(BillingWebhookRequests.ExternalPro(externalSubscriptionId, business.Business.OrganizationId));
        var eventId = BillingWebhookRequests.NewEventId();
        using var stripe = fixture.ApiFactory.CreateClient();
        using var firstResponse = await stripe.PostStripeEventAsync(BillingWebhookRequests.CheckoutSessionCompleted, externalSubscriptionId, eventId);

        using var secondResponse = await stripe.PostStripeEventAsync(BillingWebhookRequests.CheckoutSessionCompleted, externalSubscriptionId, eventId);

        secondResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        fixture.ApiFactory.BillingGateway.ReadCount(externalSubscriptionId).ShouldBe(1);
    }
}
