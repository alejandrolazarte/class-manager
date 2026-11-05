using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_stripe_sends_two_events_of_a_checkout_at_once;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_both_are_applied_without_errors(ApiFixture fixture)
{
    private const int ConcurrentEvents = 2;

    [Fact]
    public async Task Then_both_are_applied_without_errors_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var externalSubscriptionId = BillingWebhookRequests.NewExternalSubscriptionId();
        fixture.ApiFactory.BillingGateway.Set(BillingWebhookRequests.ExternalPro(externalSubscriptionId, business.Business.OrganizationId));
        fixture.ApiFactory.BillingGateway.HoldReadsUntilConcurrent(externalSubscriptionId, ConcurrentEvents);

        var checkoutCompleted = fixture.PostStripeEventAsync(BillingWebhookRequests.CheckoutSessionCompleted, externalSubscriptionId);
        var subscriptionCreated = fixture.PostStripeEventAsync(BillingWebhookRequests.SubscriptionCreated, externalSubscriptionId);
        using var checkoutCompletedResponse = await checkoutCompleted;
        using var subscriptionCreatedResponse = await subscriptionCreated;

        new[] { checkoutCompletedResponse.StatusCode, subscriptionCreatedResponse.StatusCode }.ShouldAllBe(statusCode => statusCode == HttpStatusCode.OK);
        var current = await fixture.CurrentSubscriptionAsync(business);
        current.ExternalSubscriptionId.ShouldBe(externalSubscriptionId);
    }
}
