namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_an_operator_gives_a_brand_a_sales_fee;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_other_brands_keep_their_plan_fee(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_other_brands_keep_their_plan_fee_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);
        var otherBrand = await BrandChanges.OnProAsync(fixture);

        using var response = await changes.GiveSalesFeeAsync(5m);

        (await otherBrand.GetSalesFeeAsync()).IsBrandFee.ShouldBeFalse();
    }
}
