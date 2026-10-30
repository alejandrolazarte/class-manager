using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_brand_with_an_offer_reads_the_subscription;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_offer_is_shown(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_offer_is_shown_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        var lastDay = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddDays(7);
        await fixture.OfferPlanAsync(brand.OrganizationId, PlanCodes.Lite, expiredOn: lastDay);

        var subscription = await brand.HttpClient.ReadOrganizationSubscriptionAsync();

        subscription.Offer.ShouldBe(new PlanOfferResponse(PlanCodes.Lite, SelfServiceBillingRequests.OfferedPrice, SelfServiceBillingRequests.Euro, lastDay));
    }
}
