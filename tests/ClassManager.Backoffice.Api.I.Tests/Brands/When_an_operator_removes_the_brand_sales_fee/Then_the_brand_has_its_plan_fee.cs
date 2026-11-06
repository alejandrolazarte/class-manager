namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_an_operator_removes_the_brand_sales_fee;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_has_its_plan_fee(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_the_brand_has_its_plan_fee_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);
        using var feeResponse = await changes.GiveSalesFeeAsync(5m);

        using var response = await changes.RemoveSalesFeeAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await changes.GetSalesFeeAsync()).IsBrandFee.ShouldBeFalse();
    }
}
