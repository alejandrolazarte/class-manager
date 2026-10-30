using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_the_brand_has_no_self_service_billing;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403_and_starts_no_checkout(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_and_starts_no_checkout_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);

        using var response = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Pro);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(FeatureErrorCodes.NotInPlan);
        problem.GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.SelfServiceBilling);
        fixture.ApiFactory.BillingGateway.CheckoutsFor(brand.OrganizationId).ShouldBeEmpty();
    }
}
