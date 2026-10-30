using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_trial_brand_in_spain_starts_a_checkout;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_first_charge_is_when_the_trial_ends(ApiFixture fixture)
{
    private const int TrialDays = 30;

    [Fact]
    public async Task Then_the_first_charge_is_when_the_trial_ends_Run()
    {
        var brand = await fixture.SignUpBrandAsync();

        using var response = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Lite);

        var dayAfterTheTrial = DateOnly.FromDateTime(BusinessApiFactory.Now.UtcDateTime).AddDays(TrialDays);
        fixture.ApiFactory.BillingGateway.CheckoutsFor(brand.OrganizationId).Single().FirstChargeAt
            .ShouldBe(new DateTimeOffset(dayAfterTheTrial.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero));
    }
}
