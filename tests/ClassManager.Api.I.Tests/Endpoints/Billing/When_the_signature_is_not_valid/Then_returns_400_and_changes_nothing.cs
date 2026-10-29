using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_the_signature_is_not_valid;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400_and_changes_nothing(ApiFixture fixture)
{
    private const string WrongWebhookSecret = "whsec_someone-else";

    [Fact]
    public async Task Then_returns_400_and_changes_nothing_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var externalSubscriptionId = BillingWebhookRequests.NewExternalSubscriptionId();
        fixture.ApiFactory.BillingGateway.Set(BillingWebhookRequests.ExternalPro(externalSubscriptionId, business.Business.OrganizationId));
        using var stranger = fixture.ApiFactory.CreateClient();

        using var response = await stranger.PostStripeEventAsync(
            BillingWebhookRequests.CheckoutSessionCompleted,
            externalSubscriptionId,
            webhookSecret: WrongWebhookSecret);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await fixture.CurrentSubscriptionAsync(business)).PlanCode.ShouldBe(PlanCodes.Free);
    }
}
