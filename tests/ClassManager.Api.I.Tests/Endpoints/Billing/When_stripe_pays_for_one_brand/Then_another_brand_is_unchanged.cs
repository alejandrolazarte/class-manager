using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_stripe_pays_for_one_brand;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_another_brand_is_unchanged(ApiFixture fixture)
{
    [Fact]
    public async Task Then_another_brand_is_unchanged_Run()
    {
        var payingBrand = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var otherBrand = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var externalSubscriptionId = BillingWebhookRequests.NewExternalSubscriptionId();
        fixture.ApiFactory.BillingGateway.Set(BillingWebhookRequests.ExternalPro(externalSubscriptionId, payingBrand.Business.OrganizationId));
        using var stripe = fixture.ApiFactory.CreateClient();

        using var response = await stripe.PostStripeEventAsync(BillingWebhookRequests.CheckoutSessionCompleted, externalSubscriptionId);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var otherSubscription = await fixture.CurrentSubscriptionAsync(otherBrand);
        otherSubscription.PlanCode.ShouldBe(PlanCodes.Free);
        otherSubscription.ExternalSubscriptionId.ShouldBeNull();
    }
}
