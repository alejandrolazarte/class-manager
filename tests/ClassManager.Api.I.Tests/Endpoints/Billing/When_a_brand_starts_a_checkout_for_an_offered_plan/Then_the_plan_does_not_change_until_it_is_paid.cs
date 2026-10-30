using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_brand_starts_a_checkout_for_an_offered_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_plan_does_not_change_until_it_is_paid(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_plan_does_not_change_until_it_is_paid_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await fixture.OfferPlanAsync(brand.OrganizationId, PlanCodes.Lite);

        using var response = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Lite);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await fixture.CurrentSubscriptionAsync(brand.OrganizationId)).PlanCode.ShouldBe(PlanCodes.Free);
        (await fixture.CurrentOfferAsync(brand.OrganizationId)).ShouldNotBeNull();
    }
}
