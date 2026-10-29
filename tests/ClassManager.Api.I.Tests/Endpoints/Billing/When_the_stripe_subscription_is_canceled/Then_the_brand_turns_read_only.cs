using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;
using ClassManager.Subscriptions.Billing;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_the_stripe_subscription_is_canceled;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_turns_read_only(ApiFixture fixture)
{
    private const int DaysSinceItStarted = 40;

    [Fact]
    public async Task Then_the_brand_turns_read_only_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var externalSubscriptionId = BillingWebhookRequests.NewExternalSubscriptionId();
        await fixture.StartPayingThroughStripeAsync(business, externalSubscriptionId, DaysSinceItStarted);
        var yesterday = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddDays(-1);
        fixture.ApiFactory.BillingGateway.Set(BillingWebhookRequests.ExternalPro(
            externalSubscriptionId,
            business.Business.OrganizationId,
            ExternalSubscriptionStatus.Canceled,
            endedOn: yesterday));
        using var stripe = fixture.ApiFactory.CreateClient();
        using var deletedResponse = await stripe.PostStripeEventAsync(BillingWebhookRequests.SubscriptionDeleted, externalSubscriptionId);

        using var response = await business.HttpClient.PostClientAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(SubscriptionErrorCodes.Inactive);
    }
}
