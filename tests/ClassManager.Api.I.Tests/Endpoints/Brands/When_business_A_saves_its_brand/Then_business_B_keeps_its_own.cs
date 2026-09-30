namespace ClassManager.Api.I.Tests.Endpoints.Brands.When_business_A_saves_its_brand;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_business_B_keeps_its_own(ApiFixture fixture)
{
    [Fact]
    public async Task Then_business_B_keeps_its_own_Run()
    {
        var businessA = await fixture.SeedBusinessAsync();
        var businessB = await fixture.SeedBusinessAsync();

        (await businessA.HttpClient.PutBrandAsync(BrandRequests.DeltaBrand())).EnsureSuccessStatusCode();
        (await businessA.HttpClient.PutBrandLogoAsync(BrandRequests.PngLogo)).EnsureSuccessStatusCode();

        var brandB = await businessB.HttpClient.GetBrandAsync();
        brandB!.ThemeColor.ShouldBeNull();
        brandB.DisplayName.ShouldBe(businessB.Business.Name);
        using var logoB = await businessB.HttpClient.GetBrandLogoAsync();
        logoB.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
