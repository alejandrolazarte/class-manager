namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_a_sales_fee_is_over_100_percent;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);

        using var response = await changes.GiveSalesFeeAsync(101m);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
