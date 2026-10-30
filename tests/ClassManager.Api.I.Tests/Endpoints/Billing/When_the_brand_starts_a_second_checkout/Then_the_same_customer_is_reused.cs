using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_the_brand_starts_a_second_checkout;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_same_customer_is_reused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_same_customer_is_reused_Run()
    {
        var brand = await fixture.SignUpBrandAsync();
        using var firstResponse = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Lite);

        using var secondResponse = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Pro);

        secondResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var gateway = fixture.ApiFactory.BillingGateway;
        gateway.CustomersCreatedFor(brand.OrganizationId).Count.ShouldBe(1);
        gateway.CheckoutsFor(brand.OrganizationId).Select(checkout => checkout.ExternalCustomerId).Distinct().Count().ShouldBe(1);
    }
}
