using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_the_trial_has_ended_and_the_brand_starts_a_checkout;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_first_charge_is_immediate(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_first_charge_is_immediate_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await fixture.EndTrialAsync(brand.OrganizationId);
        await fixture.GiveSelfServiceBillingAsync(brand.OrganizationId);

        using var response = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Pro);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        fixture.ApiFactory.BillingGateway.CheckoutsFor(brand.OrganizationId).Single().FirstChargeAt.ShouldBeNull();
    }
}
