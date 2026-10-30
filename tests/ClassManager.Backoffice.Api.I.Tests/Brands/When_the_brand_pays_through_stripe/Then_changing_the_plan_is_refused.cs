using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_the_brand_pays_through_stripe;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_changing_the_plan_is_refused(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_changing_the_plan_is_refused_Run()
    {
        var changes = await BrandChanges.InSpainAsync(
            fixture,
            brandId => [Subscription.StartExternal(brandId, "pro", 39m, BrandChanges.Currency, $"sub_{TestIds.Next()}", BackofficeApiFactory.Now.AddDays(-3))]);

        using var response = await changes.ChangePlanAsync("lite");

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await changes.GetBrandAsync()).Subscription!.PlanCode.ShouldBe("pro");
    }
}
