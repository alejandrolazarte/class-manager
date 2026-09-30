using ClassManager.Core.UseCases.Brands;

namespace ClassManager.Api.I.Tests.Endpoints.Brands.When_owner_saves_the_brand;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_get_brand_returns_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_get_brand_returns_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        (await business.HttpClient.PutBrandAsync(BrandRequests.DeltaBrand())).EnsureSuccessStatusCode();

        var brand = await business.HttpClient.GetBrandAsync();
        brand.ShouldBe(new BrandResponse("Club Delta", "Club Delta", "#0076b4", "#efb062", LocksTheme: true, LogoUpdatedAt: null));
    }
}
