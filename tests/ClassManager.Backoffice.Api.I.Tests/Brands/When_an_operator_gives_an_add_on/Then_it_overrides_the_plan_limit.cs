namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_an_operator_gives_an_add_on;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_overrides_the_plan_limit(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_it_overrides_the_plan_limit_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);
        (await changes.GetBrandAsync()).Features.ShouldContain(new BrandFeatureResponse("branches", 1));

        await changes.GiveAddOnAsync("branches", limit: 5);

        (await changes.GetBrandAsync()).Features.ShouldContain(new BrandFeatureResponse("branches", 5));
    }
}
