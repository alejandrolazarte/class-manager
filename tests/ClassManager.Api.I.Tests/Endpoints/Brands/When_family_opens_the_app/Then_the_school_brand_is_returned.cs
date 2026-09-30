namespace ClassManager.Api.I.Tests.Endpoints.Brands.When_family_opens_the_app;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_school_brand_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_school_brand_is_returned_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        (await owner.PutBrandAsync(BrandRequests.DeltaBrand())).EnsureSuccessStatusCode();
        (await owner.PutBrandLogoAsync(BrandRequests.PngLogo)).EnsureSuccessStatusCode();

        var brand = await scenario.Family.GetFamilyBrandAsync();

        brand!.ThemeColor.ShouldBe("#0076b4");
        using var logo = await scenario.Family.GetFamilyBrandLogoAsync();
        (await logo.Content.ReadAsByteArrayAsync()).ShouldBe(BrandRequests.PngLogo);
    }
}
