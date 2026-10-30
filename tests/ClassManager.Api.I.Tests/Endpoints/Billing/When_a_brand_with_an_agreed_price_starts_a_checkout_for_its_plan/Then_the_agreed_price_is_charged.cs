using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Subscribers;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_brand_with_an_agreed_price_starts_a_checkout_for_its_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_agreed_price_is_charged(ApiFixture fixture)
{
    private const decimal AgreedPrice = 29m;

    [Fact]
    public async Task Then_the_agreed_price_is_charged_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        await using (var context = fixture.CreateDbContext(0L))
        {
            var trial = await context.Subscriptions.SingleAsync(subscription => subscription.SubscriberId == brand.OrganizationId && !subscription.IsDeleted);
            trial.Delete(BusinessApiFactory.Now);
            context.Subscriptions.Add(Subscription.Start(
                brand.OrganizationId,
                PlanCodes.Pro,
                AgreedPrice,
                SelfServiceBillingRequests.Euro,
                note: null,
                BusinessApiFactory.Now));
            await context.SaveChangesAsync();
        }

        await fixture.GiveSelfServiceBillingAsync(brand.OrganizationId);

        using var response = await brand.HttpClient.PostPlanCheckoutAsync(PlanCodes.Pro);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        fixture.ApiFactory.BillingGateway.CheckoutsFor(brand.OrganizationId).Single().Plan.UnitAmount.ShouldBe(AgreedPrice);
    }
}
