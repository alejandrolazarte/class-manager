namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_an_operator_changes_a_plan_sales_fee;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_catalog_shows_it(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_the_catalog_shows_it_Run()
    {
        const decimal LiteSalesFee = 1.5m;
        var changes = await BrandChanges.OnProAsync(fixture);

        using var response = await changes.ChangePlanSalesFeeAsync("lite", LiteSalesFee);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await changes.GetCatalogAsync()).Plans.Single(plan => plan.Code == "lite").SalesFeePercent.ShouldBe(LiteSalesFee);
    }
}
