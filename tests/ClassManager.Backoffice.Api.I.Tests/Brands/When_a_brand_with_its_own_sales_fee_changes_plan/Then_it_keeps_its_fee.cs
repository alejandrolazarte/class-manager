namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_a_brand_with_its_own_sales_fee_changes_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_keeps_its_fee(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_it_keeps_its_fee_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);
        using var feeResponse = await changes.GiveSalesFeeAsync(5m);

        using var response = await changes.ChangePlanAsync("enterprise", 0m);

        (await changes.GetSalesFeeAsync()).Percent.ShouldBe(5m);
    }
}
