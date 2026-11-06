namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_an_operator_gives_a_brand_a_sales_fee;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_has_it(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_the_brand_has_it_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);

        using var response = await changes.GiveSalesFeeAsync(5m);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var salesFee = await changes.GetSalesFeeAsync();
        (salesFee.Percent, salesFee.IsBrandFee).ShouldBe((5m, true));
    }
}
