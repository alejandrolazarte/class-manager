namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_the_brand_has_no_sales_fee_of_its_own;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_has_its_plan_fee(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_it_has_its_plan_fee_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);
        var proSalesFee = (await changes.GetCatalogAsync()).Plans.Single(plan => plan.Code == "pro").SalesFeePercent;

        var salesFee = await changes.GetSalesFeeAsync();

        (salesFee.Percent, salesFee.IsBrandFee).ShouldBe((proSalesFee, false));
    }
}
