using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_branch_owner_starts_a_checkout_for_an_offered_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await fixture.OfferPlanAsync(brand.OrganizationId, PlanCodes.Lite);
        using var branchOwner = await fixture.SeedMemberAsync(brand.BusinessId, BusinessRole.BranchOwner);

        using var response = await branchOwner.PostPlanCheckoutAsync(PlanCodes.Lite);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        fixture.ApiFactory.BillingGateway.CheckoutsFor(brand.OrganizationId).ShouldBeEmpty();
    }
}
