namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_an_operator_changes_the_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_shows_the_new_plan_without_the_previous_add_ons(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_the_brand_shows_the_new_plan_without_the_previous_add_ons_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);
        await changes.GiveAddOnAsync("branches", limit: 5);

        using var response = await changes.ChangePlanAsync("lite");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var brand = await changes.GetBrandAsync();
        brand.Subscription.ShouldBe(new BrandSubscriptionResponse("lite", 12m, BrandChanges.Currency));
        brand.AddOns.ShouldBeEmpty();
        brand.PastSubscriptions.ShouldHaveSingleItem().PlanCode.ShouldBe("pro");
    }
}
