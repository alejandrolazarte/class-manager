using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_brand_outside_europe_starts_a_checkout;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403_not_available(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_not_available_Run()
    {
        var brand = await fixture.SignUpBrandAsync(ApiFixture.ArgentinaCountryCode);

        using var response = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Pro);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(BillingErrorCodes.NotAvailable);
    }
}
