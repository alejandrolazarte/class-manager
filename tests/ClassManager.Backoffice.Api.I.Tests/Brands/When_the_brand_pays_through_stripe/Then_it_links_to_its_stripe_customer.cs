using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_the_brand_pays_through_stripe;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_links_to_its_stripe_customer(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_it_links_to_its_stripe_customer_Run()
    {
        var changes = await BrandChanges.InSpainAsync(
            fixture,
            brandId => [Subscription.StartExternal(brandId, "pro", 39m, BrandChanges.Currency, $"sub_{TestIds.Next()}", BackofficeApiFactory.Now.AddDays(-3))]);
        var externalCustomerId = $"cus_{TestIds.Next()}";
        await fixture.SeedBillingCustomerAsync(changes.Brand.Id, externalCustomerId);

        var brand = await changes.GetBrandAsync();

        brand.BillingCustomerUrl.ShouldBe($"https://dashboard.stripe.com/customers/{externalCustomerId}");
    }
}
