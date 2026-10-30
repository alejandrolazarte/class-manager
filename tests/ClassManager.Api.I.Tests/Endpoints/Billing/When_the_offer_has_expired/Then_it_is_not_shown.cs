using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_the_offer_has_expired;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_shown(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_shown_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        var yesterday = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddDays(-1);
        await fixture.OfferPlanAsync(brand.OrganizationId, PlanCodes.Lite, expiredOn: yesterday, createdDaysAgo: 5);

        var subscription = await brand.HttpClient.ReadOrganizationSubscriptionAsync();

        subscription.Offer.ShouldBeNull();
    }
}
